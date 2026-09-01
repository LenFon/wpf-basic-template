namespace __APP_NAME__.Domain.Models;

/// <summary>
/// 消息项领域模型（示例）。
/// </summary>
/// <param name="Content">消息内容。</param>
/// <param name="CreatedAt">创建时间。</param>
public sealed partial record MessageItem(string Content, DateTime CreatedAt)
{
    /// <summary>
    /// C# 13 分部属性 —— <b>定义声明</b>：只声明签名，不写实现。
    /// 实现声明见 <c>MessageItem.Impl.cs</c>。
    /// </summary>
    public partial bool IsToday { get; }

    /// <summary>
    /// 同样是定义声明，实现声明里可带完整访问器逻辑。
    /// </summary>
    public partial string DisplayTime { get; }
}
