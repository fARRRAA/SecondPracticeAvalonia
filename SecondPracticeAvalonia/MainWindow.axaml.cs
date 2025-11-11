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
        ShowPage(new LoginPage(), "EduTrack � ����");
    }
    public void ShowPage(Control page, string? title = null)
    {
        _pageHost.Content = page;
        if (!string.IsNullOrWhiteSpace(title))
            Title = title;
    }
    public void Navigate(UserControl page, string title)
    {
        _pageHost.Content = page;
        Title = title;
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}