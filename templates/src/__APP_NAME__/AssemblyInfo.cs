using System.Runtime.Versioning;
using System.Windows;

// 声明最低支持的 Windows 平台版本，消除 Prism（net6.0-windows7.0）API 的 CA1416 警告
[assembly: SupportedOSPlatform("windows7.0")]

// 主题字典查找策略：特定主题字典在外部，通用字典在本程序集
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
