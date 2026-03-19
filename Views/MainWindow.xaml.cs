using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Win11Optimizer.Helpers;
using Win11Optimizer.Models;

namespace Win11Optimizer.Views;

public partial class MainWindow : Window
{
    private readonly TweakEngine      _engine     = new();
    private          List<TweakCategory> _categories = [];
    private          bool             _isApplying = false;

    public bool IsApplying
    {
        get => _isApplying;
        private set
        {
            _isApplying = value;
            Dispatcher.InvokeAsync(() =>
            {
                if (MainProgressBar.Parent is Border border)
                    border.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            });
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                           .IsInRole(WindowsBuiltInRole.Administrator);
        if (!isAdmin)
        {
            AdminBadge.Text       = "⚠ 管理者権限なし";
            AdminBadge.Foreground = System.Windows.Media.Brushes.OrangeRed;
            MessageBox.Show("管理者権限で起動してください。\nレジストリの変更ができません。",
                            "権限エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        SetStatus("ツイーク定義を読み込んでいます...");
        _categories = TweakDefinitions.GetAllCategories();

        CategoryNav.Items.Clear();
        foreach (var cat in _categories)
            CategoryNav.Items.Add(new ListBoxItem { Content = cat.Name, Tag = cat });

        if (CategoryNav.Items.Count > 0) CategoryNav.SelectedIndex = 0;

        SetStatus("現在の設定をスキャンしています...");
        ShowProgressBar(true);

        var progress = new Progress<(int Current, int Total, string Message)>(p =>
            Dispatcher.InvokeAsync(() =>
            {
                MainProgressBar.Maximum = p.Total;
                MainProgressBar.Value   = p.Current;
                ProgressLabel.Text      = p.Message;
            }));

        await _engine.ScanAllAsync(_categories, progress);

        TweakList.ItemsSource = _categories;
        ShowProgressBar(false);
        SetStatus($"準備完了 — {_categories.Sum(c => c.Items.Count)} 項目を読み込みました");
    }

    // ─── カテゴリ切り替え ──────────────────────────────────────────

    private void CategoryNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TweakList.ItemsSource = CategoryNav.SelectedItem is ListBoxItem { Tag: TweakCategory cat }
            ? (IEnumerable<TweakCategory>)[cat]
            : _categories;
    }

    // ─── 全選択 / 全解除 ──────────────────────────────────────────

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in _categories.SelectMany(c => c.Items)) item.IsEnabled = true;
    }

    private void BtnDeselectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in _categories.SelectMany(c => c.Items)) item.IsEnabled = false;
    }

    // ─── 設定を適用 ───────────────────────────────────────────────

    private async void BtnApply_Click(object sender, RoutedEventArgs e)
    {
        var selected = _categories.SelectMany(c => c.Items).ToList();
        if (selected.Count == 0) { SetStatus("適用する項目がありません"); return; }

        bool createRp = ChkRestorePoint.IsChecked == true;

        if (selected.Any(i => i.RequiresUcpdDisable && i.IsEnabled))
        {
            if (MessageBox.Show(
                    "ウィジェットボタン非表示の適用には UCPD ドライバーの無効化が必要です。\n" +
                    "無効化後にシステム再起動が必要です。続行しますか？",
                    "UCPD 無効化の確認", MessageBoxButton.YesNo, MessageBoxImage.Question)
                != MessageBoxResult.Yes) return;
        }

        if (MessageBox.Show(
                $"{selected.Count} 項目の設定を適用します。\n適用前にバックアップが自動保存されます。",
                "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question)
            != MessageBoxResult.OK) return;

        BtnApply.IsEnabled = false;
        ShowProgressBar(true);

        var progress = new Progress<(int Current, int Total, string Message)>(p =>
            Dispatcher.InvokeAsync(() =>
            {
                MainProgressBar.Maximum = p.Total;
                MainProgressBar.Value   = p.Current;
                ProgressLabel.Text      = p.Message;
                SetStatus(p.Message);
            }));

        var result = await _engine.ApplySelectedAsync(selected, progress, createRp);

        ShowProgressBar(false);
        BtnApply.IsEnabled = true;

        string msg = $"✅ {result.SuccessCount} 項目を適用しました。\n";
        if (result.BackupPath is not null)   msg += $"📁 バックアップ: {result.BackupPath}\n";
        if (result.Errors.Count  > 0)        msg += $"\n⚠ エラー ({result.Errors.Count}):\n"  + string.Join("\n", result.Errors.Take(5));
        if (result.Warnings.Count > 0)       msg += $"\n💡 警告:\n"                            + string.Join("\n", result.Warnings);

        if (result.NeedsSystemRestart)
        {
            msg += "\n\n🔄 一部の設定はシステム再起動後に反映されます。今すぐ再起動しますか？";
            if (MessageBox.Show(msg, "適用完了", MessageBoxButton.YesNo, MessageBoxImage.Information)
                == MessageBoxResult.Yes) ProcessHelper.RestartSystem();
        }
        else if (result.NeedsExplorerRestart)
        {
            msg += "\n\n🔄 一部の設定はエクスプローラー再起動後に反映されます。今すぐ再起動しますか？";
            if (MessageBox.Show(msg, "適用完了", MessageBoxButton.YesNo, MessageBoxImage.Information)
                == MessageBoxResult.Yes) await ProcessHelper.RestartExplorerAsync();
        }
        else
        {
            MessageBox.Show(msg, "適用完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        SetStatus($"適用完了: {result.SuccessCount} 項目");
    }

    // ─── 設定の復元 ───────────────────────────────────────────────

    private void BtnRestore_Click(object sender, RoutedEventArgs e)
    {
        var backups = _engine.GetBackupFiles();
        if (backups.Count == 0)
        {
            MessageBox.Show("バックアップファイルが見つかりません。", "復元",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        new RestoreDialog(backups, _engine) { Owner = this }.ShowDialog();
        SetStatus("復元ダイアログを閉じました");
    }

    // ─── 設定エクスポート ─────────────────────────────────────────

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog
        {
            Filter   = "JSON ファイル (*.json)|*.json",
            FileName = $"Win11Optimizer_Settings_{DateTime.Now:yyyyMMdd}"
        };
        if (dlg.ShowDialog() != true) return;
        _engine.ExportSettings(_categories.SelectMany(c => c.Items), dlg.FileName);
        SetStatus($"設定を保存しました: {dlg.FileName}");
        MessageBox.Show("設定を保存しました。\n次回インストール時に「設定を読込」で再適用できます。",
                        "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // ─── 設定インポート ───────────────────────────────────────────

    private void BtnImport_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog { Filter = "JSON ファイル (*.json)|*.json" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            _engine.ImportSettings(_categories.SelectMany(c => c.Items), dlg.FileName);
            SetStatus($"設定を読み込みました: {dlg.FileName}");
            MessageBox.Show("設定を読み込みました。\n「選択した項目の設定を適用」で反映してください。",
                            "読込完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"読込に失敗しました: {ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ─── ファイルピッカー（壁紙・ロック画面）──────────────────────

    private async void BtnFilePicker_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is not TweakItem item) return;

        var dlg = new OpenFileDialog { Title = item.Name, Filter = item.FilePickerFilter };
        if (dlg.ShowDialog() != true) return;

        string path = dlg.FileName;
        item.SelectedPath = System.IO.Path.GetFileName(path);
        item.IsLoading    = true;
        SetStatus($"{item.Name}: {path} を適用中...");

        try
        {
            bool   ok    = false;
            string psCmd = "";

            if (item.Id == "CustomWallpaper")
            {
                string ep = path.Replace("'", "''");
                psCmd = $"Add-Type -TypeDefinition 'using System;using System.Runtime.InteropServices;" +
                         "public class W{{[DllImport(\"user32.dll\")]public static extern bool " +
                        $"SystemParametersInfo(int a,int b,string c,int d);}}'; [W]::SystemParametersInfo(0x14,0,'{ep}',3)";
            }
            else if (item.Id == "CustomLockScreen")
            {
                string ep = path.Replace("'", "''");
                psCmd = $"$p='HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PersonalizationCSP'; " +
                         "New-Item -Path $p -Force | Out-Null; " +
                        $"Set-ItemProperty -Path $p -Name LockScreenImagePath   -Value '{ep}' -Force; " +
                        $"Set-ItemProperty -Path $p -Name LockScreenImageUrl    -Value '{ep}' -Force; " +
                         "Set-ItemProperty -Path $p -Name LockScreenImageStatus -Value 1 -Type DWORD -Force";
            }

            if (!string.IsNullOrEmpty(psCmd))
            {
                var (success, _, err) = await ProcessHelper.RunPowerShellAsync(psCmd);
                ok = success;
                if (!ok) SetStatus($"エラー: {err}");
            }

            if (ok)
            {
                SetStatus($"{item.Name}: 適用完了 — {path}");
                MessageBox.Show($"適用しました。\n{path}", item.Name, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            SetStatus($"エラー: {ex.Message}");
            MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { item.IsLoading = false; }
    }

    // ─── マウスホイール ────────────────────────────────────────────

    private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (sender is not ScrollViewer sv) return;
        sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta * 0.5);
        e.Handled = true;
    }

    // ─── ユーティリティ ────────────────────────────────────────────

    private void SetStatus(string message)
        => Dispatcher.InvokeAsync(() => StatusText.Text = message);

    private void ShowProgressBar(bool show)
        => Dispatcher.InvokeAsync(() =>
        {
            if (MainProgressBar.Parent is Border border)
                border.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (!show) { MainProgressBar.Value = 0; ProgressLabel.Text = ""; }
        });
}
