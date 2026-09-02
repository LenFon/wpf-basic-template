using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using __APP_NAME__.Domain.Models;
using __APP_NAME__.Application.Services;

namespace __APP_NAME__.ViewModels;

/// <summary>
/// 主窗口视图模型。由 Prism 的 ViewModelLocator 按命名约定自动装配：
/// Views.MainWindow -> ViewModels.MainWindowViewModel。
/// </summary>
/// <remarks>
/// 全部可通知属性均使用 C# 13 <b>分部属性（partial properties）</b>：
/// 声明处只写定义声明（public partial T X { get; set; }），
/// 由 CommunityToolkit.Mvvm 源生成器产出实现声明（SetProperty + 双向通知），
/// 因此不再需要手工维护 _xxx 支持字段。
/// </remarks>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly IMessageService _messageService;

    public MainWindowViewModel(IMessageService messageService)
    {
        _messageService = messageService;

        Messages.Add(new MessageItem(_messageService.GetWelcomeMessage(), DateTime.Now));
        UpdateStatus();
    }

    /// <summary>
    /// 窗口标题。
    /// </summary>
    [ObservableProperty]
    public partial string Title { get; set; } = "__APP_NAME__";

    /// <summary>
    /// 副标题。
    /// </summary>
    [ObservableProperty]
    public partial string Subtitle { get; set; } = "Prism 9 + Material Design 5 + CommunityToolkit.Mvvm 8";

    /// <summary>
    /// 输入框内容；变化时自动刷新 AddCommand 的可用状态。
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    public partial string InputText { get; set; } = string.Empty;

    /// <summary>
    /// 当前选中的消息。
    /// </summary>
    [ObservableProperty]
    public partial MessageItem? SelectedMessage { get; set; }

    /// <summary>
    /// 状态栏文本。
    /// </summary>
    [ObservableProperty]
    public partial string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 消息列表，绑定到 ListBox。
    /// </summary>
    public ObservableCollection<MessageItem> Messages { get; } = new();

    /// <summary>
    /// 分部属性带来的强类型变更回调：无需再遵循 On&lt;字段名&gt;Changed 的命名约定，
    /// 直接按属性名声明 partial 方法即可。
    /// </summary>
    partial void OnSelectedMessageChanged(MessageItem? value) => UpdateStatus();

    /// <summary>
    /// 添加一条消息；输入为空时命令自动不可用。
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void Add()
    {
        var item = new MessageItem(InputText.Trim(), DateTime.Now);
        Messages.Add(item);
        SelectedMessage = item;
        InputText = string.Empty;
        UpdateStatus();
    }

    private bool CanAdd() => !string.IsNullOrWhiteSpace(InputText);

    /// <summary>
    /// 清空列表。
    /// </summary>
    [RelayCommand]
    private void Clear()
    {
        Messages.Clear();
        SelectedMessage = null;
        UpdateStatus();
    }

    /// <summary>
    /// 异步加载示例数据，演示 async 命令。
    /// </summary>
    [RelayCommand]
    private async Task LoadSampleAsync()
    {
        foreach (var text in await _messageService.GetSamplesAsync())
        {
            Messages.Add(new MessageItem(text, DateTime.Now));
        }

        UpdateStatus();
    }

    private void UpdateStatus() => StatusText = SelectedMessage is null
        ? $"共 {Messages.Count} 条消息"
        : $"共 {Messages.Count} 条消息 · 已选中「{SelectedMessage.Content}」";
}
