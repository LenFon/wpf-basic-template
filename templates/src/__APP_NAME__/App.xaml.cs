using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Serilog;
using __APP_NAME__.Application.Services;
using __APP_NAME__.Infrastructure.Services;
using __APP_NAME__.Views;

namespace __APP_NAME__;

/// <summary>
/// Prism 引导程序。App.xaml 根元素为 prism:PrismApplication，
/// 启动时由基类自动完成容器初始化与 Shell 创建，无需手动调用 Initialize()。
/// </summary>
public partial class App : PrismApplication
{
    /// <summary>
    /// 创建主窗口（Shell）。
    /// </summary>
    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    /// <summary>
    /// 启动早期配置 Serilog 并挂接全局异常钩子（须在 Shell 创建前，见 App.GlobalException.cs）。
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        ConfigureLogging();
        AttachGlobalExceptionHandlers();
        base.OnStartup(e);
    }

    /// <summary>
    /// 退出前冲刷并关闭 Serilog。
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    /// <summary>
    /// 注册服务与导航。契约在 Application 层，实现在 Infrastructure 层。
    /// </summary>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IMessageService, MessageService>();
    }
}
