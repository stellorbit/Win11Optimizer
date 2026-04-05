using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Win11Optimizer.Helpers;

/// <summary>プロセス起動ヘルパー</summary>
public static class ProcessHelper
{
    // ─── 共通ランナー ───────────────────────────────────────────

    /// <summary>PowerShell コマンドを非同期実行する</summary>
    public static Task<(bool Success, string Output, string Error)> RunPowerShellAsync(string command)
        => RunCapturedAsync(
            "powershell.exe",
            $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{command.Replace("\"", "\\\"")}\"",
            utf8: true);

    /// <summary>任意のコマンドを非同期実行する</summary>
    public static Task<(bool Success, string Output, string Error)> RunCommandAsync(string exe, string args)
        => RunCapturedAsync(exe, args, utf8: false);

    private static Task<(bool Success, string Output, string Error)> RunCapturedAsync(
        string exe, string args, bool utf8)
    {
        return Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName               = exe,
                    Arguments              = args,
                    UseShellExecute        = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    CreateNoWindow         = true
                };
                if (utf8)
                {
                    psi.StandardOutputEncoding = Encoding.UTF8;
                    psi.StandardErrorEncoding  = Encoding.UTF8;
                }

                using var proc = Process.Start(psi)
                                 ?? throw new InvalidOperationException("プロセスの起動に失敗しました");
                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit();
                return (proc.ExitCode == 0, stdout.Trim(), stderr.Trim());
            }
            catch (Exception ex)
            {
                return (false, "", ex.Message);
            }
        });
    }

    // ─── システム操作 ───────────────────────────────────────────

    /// <summary>エクスプローラーを再起動する</summary>
    public static Task RestartExplorerAsync()
        => RunPowerShellAsync("Stop-Process -Name explorer -Force; Start-Sleep 1; Start-Process explorer");

    /// <summary>システムを再起動する</summary>
    public static void RestartSystem(int delaySeconds = 10)
        => Process.Start("shutdown", $"/r /t {delaySeconds} /c \"Win11Optimizer: 設定を適用するためにシステムを再起動します\"");

    /// <summary>保留中の再起動をキャンセルする</summary>
    public static void CancelRestart()
        => Process.Start("shutdown", "/a");

    // ─── UCPD ──────────────────────────────────────────────────

    /// <summary>UCPD ドライバーを無効化する（要管理者・要再起動）</summary>
    public static async Task<bool> DisableUcpdAsync()
    {
        var r1 = await RunCommandAsync("schtasks.exe",
            "/change /Disable /TN \"\\Microsoft\\Windows\\AppxDeploymentClient\\UCPD velocity\"");
        var r2 = await RunCommandAsync("sc.exe", "config UCPD start= disabled");
        return r1.Success && r2.Success;
    }

    /// <summary>UCPD ドライバーを有効化する（要管理者・要再起動）</summary>
    public static async Task<bool> EnableUcpdAsync()
    {
        var r1 = await RunCommandAsync("schtasks.exe",
            "/change /Enable /TN \"\\Microsoft\\Windows\\AppxDeploymentClient\\UCPD velocity\"");
        var r2 = await RunCommandAsync("sc.exe", "config UCPD start= auto");
        return r1.Success && r2.Success;
    }

    // ─── 復元ポイント ───────────────────────────────────────────

    /// <summary>システムの復元ポイントを作成する</summary>
    public static Task<(bool Success, string Error)> CreateRestorePointAsync(string description)
    {
        return Task.Run(() =>
        {
            try
            {
                using var restoreClass = new ManagementClass(@"\\.\root\default", "SystemRestore", null);
                using var inParams = restoreClass.GetMethodParameters("CreateRestorePoint");
                inParams["Description"] = description;
                inParams["RestorePointType"] = 0;
                inParams["EventType"] = 100;

                using var outParams = restoreClass.InvokeMethod("CreateRestorePoint", inParams, null);
                var result = Convert.ToInt32(outParams?["ReturnValue"] ?? -1);

                return result == 0
                    ? (true, "")
                    : (false, GetRestorePointErrorMessage(result));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        });
    }

    private static string GetRestorePointErrorMessage(int code) => code switch
    {
        1 => "復元ポイントの作成は既に別の処理で進行中です。",
        2 => "復元ポイントの作成に失敗しました。",
        3 => "復元ポイントの作成は無効化されています。",
        4 => "システムの復元サービスが利用できません。",
        5 => "復元ポイントの作成に対応していない構成です。",
        _ => $"復元ポイントの作成に失敗しました。(Code: {code})"
    };

    // ─── パッケージ管理 ─────────────────────────────────────────

    /// <summary>winget msstore ソースを削除する（要管理者）</summary>
    public static async Task<(bool Ok, string Message)> RemoveWingetMsStoreAsync()
    {
        var (ok, _, err) = await RunCommandAsync("winget", "source remove msstore --disable-interactivity");
        return (ok, ok ? "msstore ソースを削除しました" : err);
    }

    /// <summary>Chocolatey をインストールする</summary>
    public static async Task<(bool Ok, string Message)> InstallChocolateyAsync()
    {
        const string cmd =
            "Set-ExecutionPolicy Bypass -Scope Process -Force; " +
            "[System.Net.ServicePointManager]::SecurityProtocol = " +
            "[System.Net.ServicePointManager]::SecurityProtocol -bor 3072; " +
            "iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))";
        var (ok, _, err) = await RunPowerShellAsync(cmd);
        return (ok, ok ? "Chocolatey をインストールしました" : err);
    }

    // ─── Windows クリーンアップ ──────────────────────────────────

    /// <summary>Windows Update ファイルのクリーンアップ</summary>
    public static async Task<(bool Ok, string Message)> CleanupWindowsUpdateFilesAsync()
    {
        var (ok1, _, err1) = await RunCommandAsync("dism.exe",
            "/Online /Cleanup-Image /StartComponentCleanup /ResetBase");
        await RunCommandAsync("cleanmgr.exe", "/sageset:65535 & cleanmgr /sagerun:65535");
        var (ok3, _, _) = await RunPowerShellAsync(
            "Remove-Item -Path 'C:\\Windows\\SoftwareDistribution\\Download\\*' -Recurse -Force -ErrorAction SilentlyContinue; " +
            "Remove-Item -Path 'C:\\$Windows.~BT' -Recurse -Force -ErrorAction SilentlyContinue; " +
            "Remove-Item -Path 'C:\\$Windows.~WS' -Recurse -Force -ErrorAction SilentlyContinue");

        return ok1
            ? (true, $"DISM クリーンアップ完了。{(ok3 ? " 残骸フォルダも削除しました。" : "")}")
            : (false, $"DISM エラー: {err1}");
    }

    // ─── ウィジェット ────────────────────────────────────────────

    /// <summary>ウィジェット (WebExperience) を完全にアンインストールする</summary>
    public static async Task<(bool Ok, string Message)> UninstallWidgetsAsync()
    {
        var (ok, _, err) = await RunPowerShellAsync("Get-AppxPackage *WebExperience* | Remove-AppxPackage");
        return (ok, ok ? "Windows Web Experience Pack をアンインストールしました" : err);
    }

    /// <summary>ウィジェットを再インストールする</summary>
    public static async Task<(bool Ok, string Message)> ReinstallWidgetsAsync()
    {
        var (ok, _, err) = await RunCommandAsync("winget",
            "install --id 9MSSGKG348SP --accept-package-agreements --accept-source-agreements");
        return (ok, ok ? "Windows Web Experience Pack を再インストールしました" : err);
    }
}
