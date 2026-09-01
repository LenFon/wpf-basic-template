using System.Windows;

namespace __APP_NAME__.Views;

/// <summary>
/// 主窗口。DataContext 由 Prism 的 ViewModelLocator 自动注入。
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();
}
