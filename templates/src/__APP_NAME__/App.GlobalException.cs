using System.IO;
using System.Windows;
using System.Windows.Threading;
using Serilog;

namespace __APP_NAME__;

/// <summary>
/// 全局异常处理 + 日志配置（App 分部类）。
/// 约定：ViewModel 内不 try-catch，异常一律冒泡到这里统一收口（Serilog 记录 + 弹窗提示）。
/// 覆盖三路：UI 线程异常（Dispatcher）、进程级异常（AppDomain）、未观察 Task 异常（TaskScheduler）。
/// </summary>
public partial class App
{
    // 保护 _isShowingDialog 的检查与赋值（三钩子可能并发）
    private static readonly Lock ShowDialogLock = new();
    private static bool _isShowingDialog;

    /// <summary>
    /// 配置 Serilog：文件 sink 按天滚动，保留 14 天。须在挂接异常钩子之前调用。
    /// </summary>
    public void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: Path.Combine(LogDirectory, "app-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true,
                encoding: System.Text.Encoding.UTF8)
            .CreateLogger();
    }

    /// <summary>
    /// 挂接全局异常钩子，须在应用启动早期调用一次。
    /// </summary>
    public void AttachGlobalExceptionHandlers()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    /// <summary>
    /// UI 线程未处理异常：记录 + 提示，并拦截崩溃（应用继续运行）。
    /// </summary>
    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "[Dispatcher] UI 线程未处理异常");
        ShowErrorDialog(e.Exception, isFatal: false);
        // 阻止进程退出
        e.Handled = true;
    }

    /// <summary>
    /// 进程级未处理异常（非 UI 线程 / 致命）：记录 + 提示，随后进程退出。
    /// </summary>
    private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception ?? new Exception("未知错误（非 Exception 类型）。");
        Log.Fatal(ex, "[AppDomain] 进程级未处理异常");
        ShowErrorDialog(ex, isFatal: true);
    }

    /// <summary>
    /// 未观察 Task 异常：仅记录并标记已观察（防御性兜底，不打扰用户）。
    /// </summary>
    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(e.Exception, "[TaskScheduler] 未观察的 Task 异常");
        e.SetObserved();
    }

    /// <summary>
    /// 弹窗提示（防重复弹窗；后台线程触发时调度到 UI 线程）。
    /// </summary>
    private static void ShowErrorDialog(Exception ex, bool isFatal)
    {
        lock (ShowDialogLock)
        {
            if (_isShowingDialog)
            {
                return;
            }

            _isShowingDialog = true;
        }

        try
        {
            var message =
                $"程序发生未处理的异常：{ex.Message}{Environment.NewLine}{Environment.NewLine}" +
                $"详细信息已写入日志目录：{LogDirectory}{Environment.NewLine}{Environment.NewLine}" +
                (isFatal ? "应用即将退出，请重启。" : "应用将继续运行。");

            Action show = () =>
            {
                MessageBox.Show(message,
                    isFatal ? "程序异常（即将退出）" : "程序异常",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                _isShowingDialog = false;
            };

            // 注意：App 在 __APP_NAME__ 命名空间下，Application 会解析成 __APP_NAME__.Application 命名空间，必须全限定
            if (System.Windows.Application.Current?.Dispatcher is { } dispatcher && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(show);
            }
            else
            {
                show();
            }
        }
        catch
        {
            _isShowingDialog = false;
        }
    }

    private static string LogDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "__APP_NAME__", "logs");
}
