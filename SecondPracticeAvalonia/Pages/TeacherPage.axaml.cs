using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecondPracticeAvalonia.Data;


namespace SecondPracticeAvalonia.Pages;

public partial class TeacherPage : UserControl
{
    private readonly int _userId;
    private readonly Action _logoutAction;
    private readonly DataGrid _coursesGrid;
    private readonly DataGrid _webinarsGrid;
    private readonly TextBlock _userInfoText;

    public TeacherPage(int userId, Action logoutAction)
    {
        _userId = userId;
        _logoutAction = logoutAction;
        InitializeComponent();

        _coursesGrid = this.FindControl<DataGrid>("CoursesGrid")!;
        _webinarsGrid = this.FindControl<DataGrid>("WebinarsGrid")!;
        _userInfoText = this.FindControl<TextBlock>("UserInfoText")!;

        _userInfoText.Text = $"ID: {_userId}";
        _ = LoadCoursesAsync();
    }

    private async Task LoadCoursesAsync()
    {
        _coursesGrid.IsVisible = true;
        _webinarsGrid.IsVisible = false;

        using var db = new AppDbContext();
        var data = await Task.Run(() =>
            (from ct in db.CourseTeachers
             join c in db.Courses on ct.CourseId equals c.Id
             join cat in db.CourseCategories on c.CategoryId equals cat.Id into cc
             from cat in cc.DefaultIfEmpty()
             where ct.TeacherId == _userId
             orderby c.Title
             select new
             {
                 c.Title,
                 CategoryName = cat != null ? cat.Name : "",
                 c.TotalStudents
             }).ToList());

        _coursesGrid.ItemsSource = data;
    }

    private async Task LoadWebinarsAsync()
    {
        _coursesGrid.IsVisible = false;
        _webinarsGrid.IsVisible = true;

        using var db = new AppDbContext();
        var data = await Task.Run(() =>
            (from w in db.Webinars
             join c in db.Courses on w.CourseId equals c.Id
             where w.TeacherId == _userId
             orderby w.ScheduledAt
             select new
             {
                 w.Title,
                 CourseTitle = c.Title,
                 w.ScheduledAt
             }).ToList());

        _webinarsGrid.ItemsSource = data;
    }

    private async void OnCoursesClick(object? sender, RoutedEventArgs e)
    {
        await LoadCoursesAsync();
    }

    private async void OnWebinarsClick(object? sender, RoutedEventArgs e)
    {
        await LoadWebinarsAsync();
    }

    private async void OnLessonsClick(object? sender, RoutedEventArgs e)
    {
        var manageLessonsWindow = new Windows.ManageLessons(_userId);
        await manageLessonsWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
    }

    private async void OnTestsClick(object? sender, RoutedEventArgs e)
    {
        var manageTestsWindow = new Windows.ManageTests(_userId);
        await manageTestsWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
    }

    private void OnLogoutClick(object? sender, RoutedEventArgs e)
    {
        _logoutAction?.Invoke();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}


