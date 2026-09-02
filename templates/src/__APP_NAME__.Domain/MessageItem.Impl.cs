namespace __APP_NAME__.Domain.Models;

/// <summary>
/// <see cref="MessageItem"/> 的分部实现，承载分部属性的<b>实现声明</b>。
/// </summary>
public sealed partial record MessageItem
{
    /// <summary>
    /// 实现声明：定义与声明处的访问器必须一一对应（此处只有 get）。
    /// </summary>
    public partial bool IsToday => CreatedAt.Date == DateTime.Today;

    public partial string DisplayTime => IsToday
        ? $"今天 {CreatedAt:HH:mm:ss}"
        : CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
}
