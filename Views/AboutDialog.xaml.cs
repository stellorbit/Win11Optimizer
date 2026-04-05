using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Win11Optimizer.Views;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });

        e.Handled = true;
    }
}
