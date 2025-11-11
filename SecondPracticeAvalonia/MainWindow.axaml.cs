using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SecondPracticeAvalonia;

public partial class MainWindow : Window
{
    private readonly ContentControl _pageHost;

    public MainWindow()
    {
        InitializeComponent();
        _pageHost = this.FindControl<ContentControl>("PageHost")!;
    }

    public void ShowPage(Control page, string? title = null)
    {
        _pageHost.Content = page;
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}