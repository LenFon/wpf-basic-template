namespace __APP_NAME__.Application.Services;

/// <summary>
/// 消息服务契约。
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// 获取启动欢迎消息。
    /// </summary>
    string GetWelcomeMessage();

    /// <summary>
    /// 异步获取示例数据。
    /// </summary>
    Task<IReadOnlyList<string>> GetSamplesAsync();
}
