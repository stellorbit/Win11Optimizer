using System.Windows;

namespace Win11Optimizer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // グローバル例外ハンドラ
        DispatcherUnhandledException += (_, ex) =>
        {
            MessageBox.Show(
                $"予期しないエラーが発生しました:\n{ex.Exception.Message}\n\n{ex.Exception.StackTrace}",
                "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            ex.Handled = true;
        };
    }
}
