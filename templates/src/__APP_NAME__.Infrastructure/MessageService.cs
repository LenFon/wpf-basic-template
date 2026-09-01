using __APP_NAME__.Application.Services;

namespace __APP_NAME__.Infrastructure.Services;

/// <summary>
/// 消息服务默认实现（由 Prism 容器以单例注册）。
/// </summary>
public sealed class MessageService : IMessageService
{
    public string GetWelcomeMessage()
        => $"应用已启动 · {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

    public Task<IReadOnlyList<string>> GetSamplesAsync()
    {
        IReadOnlyList<string> samples =
        [
            "上料模块：基板 / 飞达 / 振动筛",
            "视觉算法：定位与检测",
            "运控模块：WMX3 轴控"
        ];

        return Task.FromResult(samples);
    }
}
