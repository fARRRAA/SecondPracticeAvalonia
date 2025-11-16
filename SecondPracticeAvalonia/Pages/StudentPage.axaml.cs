using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecondPracticeAvalonia.Data;
using SecondPracticeAvalonia.Windows;


namespace SecondPracticeAvalonia.Pages;

public partial class StudentPage : UserControl
{
    private readonly int _userId;
    private readonly Action _logoutAction;
    private readonly DataGrid _coursesGrid;
    private readonly DataGrid _progressGrid;
    private readonly TextBlock _userInfoText;
    private readonly ContentControl _contentArea;

    public StudentPage(int userId, Action logoutAction)
    {
        _userId = userId;
        _logoutAction = logoutAction;
        InitializeComponent();

        _coursesGrid = this.FindControl<DataGrid>("CoursesGrid")!;
        _progressGrid = this.FindControl<DataGrid>("ProgressGrid")!;
        _userInfoText = this.FindControl<TextBlock>("UserInfoText")!;
        _contentArea = this.FindControl<ContentControl>("ContentArea")!;

        _userInfoText.Text = $"ID: {_userId}";
        _ = LoadCoursesAsync();
    }

    private async Task LoadCoursesAsync()
    {
        _coursesGrid.IsVisible = true;
        _progressGrid.IsVisible = false;
        _contentArea.IsVisible = false;

        using var db = new AppDbContext();
        var data = await Task.Run(() =>
            (from ce in db.CourseEnrollments
             join c in db.Courses on ce.CourseId equals c.Id
             join cat in db.CourseCategories on c.CategoryId equals cat.Id into cc
             from cat in cc.DefaultIfEmpty()
             where ce.StudentId == _userId
             orderby c.Title
             select new
             {
                 c.Title,
                 CategoryName = cat != null ? cat.Name : "",
                 Progress = ce.ProgressPercentage,
                 CourseId = c.Id
             }).ToList());

        _coursesGrid.ItemsSource = data;
    }

    private async Task LoadProgressAsync()
    {
        _coursesGrid.IsVisible = false;
        _progressGrid.IsVisible = true;
        _contentArea.IsVisible = false;

        using var db = new AppDbContext();
        var data = await Task.Run(() =>
            (from lp in db.LessonProgresses
             join l in db.Lessons on lp.LessonId equals l.Id
             join m in db.CourseModules on l.ModuleId equals m.Id
             join c in db.Courses on m.CourseId equals c.Id
             where lp.StudentId == _userId
             orderby c.Title, l.Title
             select new
             {
                 LessonTitle = l.Title,
                 CourseTitle = c.Title,
                 lp.IsCompleted,
                 lp.LastPositionSeconds
             }).ToList());

        _progressGrid.ItemsSource = data;
    }

    private async void OnCoursesClick(object? sender, RoutedEventArgs e)
    {
        await LoadCoursesAsync();
    }

    private async void OnProgressClick(object? sender, RoutedEventArgs e)
    {
        await LoadProgressAsync();
    }

    private void OnLogoutClick(object? sender, RoutedEventArgs e)
    {
        _logoutAction?.Invoke();
    }

    private async void OnBrowseCoursesClick(object? sender, RoutedEventArgs e)
    {
        var browseCoursesWindow = new SecondPracticeAvalonia.Windows.BrowseCourses(_userId);
        await browseCoursesWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
        await LoadCoursesAsync();
    }

    private async void OpenCourse_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int courseId)
        {
            var courseContentWindow = new SecondPracticeAvalonia.Windows.CourseContent(_userId, courseId);
            await courseContentWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
        }
    }

    private void OnDiscussionsClick(object? sender, RoutedEventArgs e)
    {
        _coursesGrid.IsVisible = false;
        _progressGrid.IsVisible = false;
        _contentArea.IsVisible = true;
        _contentArea.Content = new SecondPracticeAvalonia.Pages.SubPages.StudentDiscussionsPage(_userId);
    }

    private void OnAnnouncementsClick(object? sender, RoutedEventArgs e)
    {
        _coursesGrid.IsVisible = false;
        _progressGrid.IsVisible = false;
        _contentArea.IsVisible = true;
        _contentArea.Content = new SecondPracticeAvalonia.Pages.SubPages.StudentAnnouncementsPage(_userId);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}


