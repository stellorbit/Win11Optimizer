using System.Collections.Generic;
using System.IO;
using System.Windows;
using Win11Optimizer.Helpers;
using Win11Optimizer.Models;

namespace Win11Optimizer.Views;

public partial class RestoreDialog : Window
{
    private readonly TweakEngine   _engine;
    private readonly List<string>  _backupFiles;
    private BackupSession?         _selectedSession;

    public RestoreDialog(List<string> backupFiles, TweakEngine engine)
    {
        InitializeComponent();
        _backupFiles = backupFiles;
        _engine      = engine;
        LoadList();
    }

    private void LoadList()
    {
        BackupList.Items.Clear();
        foreach (var file in _backupFiles)
            BackupList.Items.Add(new FileListItem(file, Path.GetFileNameWithoutExtension(file)));

        BackupList.SelectionChanged += (_, _) =>
        {
            if (BackupList.SelectedItem is not FileListItem item) return;
            _selectedSession     = _engine.LoadBackup(item.FilePath);
            BtnRestore.IsEnabled = _selectedSession is not null;
            if (_selectedSession is { } s)
                BackupInfo.Text = $"作成日時: {s.CreatedAt}\n説明: {s.Description}\nエントリ数: {s.Entries.Count}";
        };
    }

    private void BtnRestore_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedSession is null) return;
        if (MessageBox.Show(
                "選択したバックアップで設定を復元します。\n現在の設定が上書きされます。続行しますか？",
                "復元の確認", MessageBoxButton.YesNo, MessageBoxImage.Warning)
            != MessageBoxResult.Yes) return;

        var (ok, fail) = _engine.RestoreFromBackup(_selectedSession);
        MessageBox.Show(
            $"復元完了\n成功: {ok}  失敗: {fail}\n\n一部の設定はエクスプローラーまたはシステムの再起動後に反映されます。",
            "復元完了", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

    // ─── 内部クラス ───────────────────────────────────────────────

    private sealed class FileListItem(string filePath, string displayName)
    {
        public string FilePath    { get; } = filePath;
        public string DisplayName { get; } = displayName;
        public override string ToString() => DisplayName;
    }
}
