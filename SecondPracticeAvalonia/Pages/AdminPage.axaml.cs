using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecondPracticeAvalonia.Models;

namespace SecondPracticeAvalonia.Pages;

public partial class AdminPage : UserControl
{
    private readonly int _userId;
    private readonly Action _logoutAction;
    private readonly DataGrid _coursesGrid;
    private readonly DataGrid _usersGrid;
    private readonly TextBlock _userInfoText;

    public AdminPage(int userId, Action logoutAction)
    {
        _userId = userId;
        _logoutAction = logoutAction;
        InitializeComponent();

        _coursesGrid = this.FindControl<DataGrid>("CoursesGrid")!;
        _usersGrid = this.FindControl<DataGrid>("UsersGrid")!;
        _userInfoText = this.FindControl<TextBlock>("UserInfoText")!;

        _userInfoText.Text = $"ID: {_userId}";
        _ = LoadCoursesAsync();
    }

    private async Task LoadCoursesAsync()
    {
        _coursesGrid.IsVisible = true;
        _usersGrid.IsVisible = false;

        using var db = new AppDbContext();
        var items = await Task.Run(() =>
            (from c in db.Courses
             join cat in db.CourseCategories on c.CategoryId equals cat.Id into cc
             from cat in cc.DefaultIfEmpty()
             orderby c.Id
             select new
             {
                 c.Id,
                 c.Title,
                 CategoryName = cat != null ? cat.Name : "",
                 c.Price,
                 c.Rating
             }).ToList());

        _coursesGrid.ItemsSource = items;
    }

    private async Task LoadUsersAsync()
    {
        _coursesGrid.IsVisible = false;
        _usersGrid.IsVisible = true;

        using var db = new AppDbContext();
        var users = await Task.Run(() =>
            db.Users
              .OrderBy(u => u.Id)
              .Select(u => new
              {
                  u.Id,
                  u.Email,
                  u.FirstName,
                  u.LastName,
                  IsActive = u.IsActive ?? false,
                  IsBlocked = u.IsBlocked ?? false
              })
              .ToList());

        _usersGrid.ItemsSource = users;
    }

    private async void OnCoursesClick(object? sender, RoutedEventArgs e)
    {
        await LoadCoursesAsync();
    }

    private async void OnUsersClick(object? sender, RoutedEventArgs e)
    {
        await LoadUsersAsync();
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


