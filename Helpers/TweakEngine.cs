using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Win11Optimizer.Models;

namespace Win11Optimizer.Helpers;

/// <summary>ツイークの適用・復元・バックアップを管理するエンジン</summary>
public class TweakEngine
{
    private static readonly string AppDataDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Win11Optimizer");
    private static readonly string BackupDir  = Path.Combine(AppDataDir, "Backups");

    public TweakEngine()
    {
        Directory.CreateDirectory(AppDataDir);
        Directory.CreateDirectory(BackupDir);
    }

    // ──────────────── 状態スキャン ────────────────

    public async Task ScanAllAsync(
        IEnumerable<TweakCategory> categories,
        IProgress<(int Current, int Total, string Message)> progress)
    {
        var allItems = categories.SelectMany(c => c.Items).ToList();
        int total = allItems.Count, current = 0;

        foreach (var item in allItems)
        {
            progress.Report((++current, total, $"スキャン中: {item.Name}"));
            await Task.Yield();
            item.IsEnabled = item.RegistryEntries.Count > 0
                ? RegistryHelper.ReadTweakCurrentState(item)
                : false;
        }
    }

    // ──────────────── 適用 ────────────────

    public async Task<TweakApplyResult> ApplySelectedAsync(
        IEnumerable<TweakItem> selectedItems,
        IProgress<(int Current, int Total, string Message)> progress,
        bool createRestorePoint = false)
    {
        var items  = selectedItems.ToList();
        var result = new TweakApplyResult();

        if (createRestorePoint)
        {
            progress.Report((0, items.Count, "復元ポイントを作成中..."));
            await ProcessHelper.CreateRestorePointAsync("Win11Optimizer 設定適用前");
        }

        var session = new BackupSession
        {
            Description = $"適用前バックアップ {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
        };

        bool needsUcpd = items.Any(i => i.RequiresUcpdDisable && i.IsEnabled);
        if (needsUcpd)
        {
            progress.Report((0, items.Count, "UCPD ドライバーを無効化中（ウィジェット対策）..."));
            result.UcpdDisabled = await ProcessHelper.DisableUcpdAsync();
            if (!result.UcpdDisabled)
                result.Warnings.Add("UCPDドライバーの無効化に失敗しました。ウィジェットボタン設定は再起動後に反映されます。");
        }

        int current = 0;
        foreach (var item in items)
        {
            progress.Report((++current, items.Count, $"適用中: {item.Name}"));
            item.IsLoading = true;
            try
            {
                if (item.RegistryEntries.Count > 0)
                    session.Entries.AddRange(RegistryHelper.CollectBackup(item.RegistryEntries));

                bool success;
                if (!string.IsNullOrEmpty(item.PowerShellEnableCommand) && item.IsEnabled)
                {
                    var (ok, _, err) = await ProcessHelper.RunPowerShellAsync(item.PowerShellEnableCommand);
                    success = ok;
                    if (!ok) result.Errors.Add($"{item.Name}: {err}");
                }
                else if (!string.IsNullOrEmpty(item.PowerShellDisableCommand) && !item.IsEnabled)
                {
                    var (ok, _, err) = await ProcessHelper.RunPowerShellAsync(item.PowerShellDisableCommand);
                    success = ok;
                    if (!ok) result.Errors.Add($"{item.Name}: {err}");
                }
                else
                {
                    success = ApplyRegistryEntries(item);
                    if (!success) result.Errors.Add($"{item.Name}: レジストリ書き込みに失敗しました");
                }

                if (success)
                {
                    result.SuccessCount++;
                    switch (item.RestartRequirement)
                    {
                        case RestartRequirement.SystemRestart:   result.NeedsSystemRestart   = true; break;
                        case RestartRequirement.ExplorerRestart: result.NeedsExplorerRestart = true; break;
                    }
                }
            }
            catch (SecurityException)
            {
                result.Errors.Add($"{item.Name}: アクセスが拒否されました（UCPDブロックの可能性）");
            }
            catch (Exception ex)
            {
                result.Errors.Add($"{item.Name}: {ex.Message}");
            }
            finally
            {
                item.IsLoading = false;
            }
        }

        if (session.Entries.Count > 0)
        {
            var path = Path.Combine(BackupDir, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(session, Formatting.Indented));
            result.BackupPath = path;
        }

        return result;
    }

    private static bool ApplyRegistryEntries(TweakItem item)
    {
        bool allOk = true;
        foreach (var entry in item.RegistryEntries)
        {
            try
            {
                if (item.IsEnabled)
                {
                    RegistryHelper.WriteValue(entry.Hive, entry.KeyPath,
                        entry.ValueName, entry.ValueData, entry.ValueType);
                }
                else if (entry.DeleteOnDisable)
                {
                    RegistryHelper.DeleteKey(entry.Hive, entry.KeyPath);
                }
                else if (!string.IsNullOrEmpty(entry.DefaultData) && entry.DefaultData != "DELETE")
                {
                    RegistryHelper.WriteValue(entry.Hive, entry.KeyPath,
                        entry.ValueName, entry.DefaultData, entry.ValueType);
                }
                else
                {
                    RegistryHelper.DeleteValue(entry.Hive, entry.KeyPath, entry.ValueName);
                }
            }
            catch { allOk = false; }
        }
        return allOk;
    }

    // ──────────────── 復元 ────────────────

    public List<string> GetBackupFiles()
        => [.. Directory.GetFiles(BackupDir, "backup_*.json").OrderDescending()];

    public BackupSession? LoadBackup(string filePath)
    {
        try { return JsonConvert.DeserializeObject<BackupSession>(File.ReadAllText(filePath)); }
        catch { return null; }
    }

    public (int Success, int Failed) RestoreFromBackup(BackupSession session)
        => RegistryHelper.RestoreBackup(session.Entries);

    // ──────────────── 設定エクスポート / インポート ────────────────

    public void ExportSettings(IEnumerable<TweakItem> items, string filePath)
    {
        var data = items.Select(i => new { i.Id, i.IsEnabled });
        File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
    }

    public void ImportSettings(IEnumerable<TweakItem> items, string filePath)
    {
        var data = JsonConvert.DeserializeObject<List<IdEnabled>>(File.ReadAllText(filePath));
        if (data is null) return;
        foreach (var item in items)
        {
            var match = data.Find(d => d.Id == item.Id);
            if (match is not null) item.IsEnabled = match.IsEnabled;
        }
    }

    private sealed class IdEnabled { public string Id { get; set; } = ""; public bool IsEnabled { get; set; } }
}

public class TweakApplyResult
{
    public int          SuccessCount         { get; set; }
    public bool         NeedsExplorerRestart { get; set; }
    public bool         NeedsSystemRestart   { get; set; }
    public bool         UcpdDisabled         { get; set; }
    public string?      BackupPath           { get; set; }
    public List<string> Errors               { get; set; } = [];
    public List<string> Warnings             { get; set; } = [];
}
