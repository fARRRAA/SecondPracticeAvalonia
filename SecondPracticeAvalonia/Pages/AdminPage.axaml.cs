using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;

namespace SecondPracticeAvalonia.Pages;

public partial class AdminPage : UserControl
{
    private readonly int _userId;
    private readonly Action _logoutAction;
    private readonly DataGrid _coursesGrid;
    private readonly DataGrid _usersGrid;
    private readonly TextBlock _userInfoText;
    private readonly StackPanel _usersPanel;
    private readonly Button _addCourseButton;
    private readonly Button _addUserButton;

    public AdminPage(int userId, Action logoutAction)
    {
        _userId = userId;
        _logoutAction = logoutAction;
        InitializeComponent();

        _coursesGrid = this.FindControl<DataGrid>("CoursesGrid")!;
        _usersGrid = this.FindControl<DataGrid>("UsersGrid")!;
        _userInfoText = this.FindControl<TextBlock>("UserInfoText")!;
        _usersPanel = this.FindControl<StackPanel>("UsersPanel")!;
        _addCourseButton = this.FindControl<Button>("AddCourse")!;
        _addUserButton = this.FindControl<Button>("AddUser")!;

        _userInfoText.Text = $"ID: {_userId}";
        _ = LoadCoursesAsync();
    }

    private async Task LoadCoursesAsync()
    {
        _coursesGrid.IsVisible = true;
        _usersPanel.IsVisible = false;
        _addCourseButton.IsVisible = true;
        _addUserButton.IsVisible = false;

        using var db = new AppDbContext();
        var courses = db.Courses.Include(x => x.Category).ToList();
        _coursesGrid.ItemsSource = courses;
    }

    private async Task LoadUsersAsync()
    {
        _coursesGrid.IsVisible = false;
        _usersPanel.IsVisible = true;
        _addCourseButton.IsVisible = false;
        _addUserButton.IsVisible = true;

        using var db = new AppDbContext();
        var users = await Task.Run(() =>
            db.Users
              .Include(x => x.Role)
              .OrderBy(u => u.Id)
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

    private async void AddCourse_Click(object? sender, RoutedEventArgs e)
    {
        var addCourseWindow = new Windows.AddOrEditCourse();
        await addCourseWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
        Refresh();
    }
    public void Refresh()
    {
        var dbContext = new AppDbContext();

        var product = dbContext.Courses.OrderBy(x => x.Id).ToList();
        _coursesGrid.ItemsSource = product;
    }

    private async void EditCourse_Click(object? sender, RoutedEventArgs e)
    {
        var course = (sender as Button).Tag as Course;
        var addCourseWindow = new Windows.AddOrEditCourse((sender as Button).Tag as Course);

        await addCourseWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
        Refresh();
    }

    private async void DeleteCourse_Click(object? sender, RoutedEventArgs e)
    {
        var ask = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Точно удалить курс?", ButtonEnum.YesNo);
        if (await ask.ShowAsync() == ButtonResult.Yes)
        {
            var course = (sender as Button).Tag as Course;
            var dbContext = new AppDbContext();
            dbContext.Courses.Remove(course);
            dbContext.SaveChanges();
            Refresh();
        }
    }

    private async void AddUser_Click(object? sender, RoutedEventArgs e)
    {
        var addUserWindow = new Windows.AddOrEditUser();
        await addUserWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
        await LoadUsersAsync();
    }

    private async void EditUser_Click(object? sender, RoutedEventArgs e)
    {
        var user = (sender as Button).Tag as User;
        var addUserWindow = new Windows.AddOrEditUser(user);
        await addUserWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
        await LoadUsersAsync();
    }

    private async void ToggleBlockUser_Click(object? sender, RoutedEventArgs e)
    {
        var user = (sender as Button).Tag as User;
        if (user == null) return;

        var action = user.IsBlocked == true ? "разблокировать" : "заблокировать";
        var ask = MessageBoxManager.GetMessageBoxStandard("Подтверждение", $"Вы уверены, что хотите {action} этого пользователя?", ButtonEnum.YesNo);
        if (await ask.ShowAsync() == ButtonResult.Yes)
        {
            var dbContext = new AppDbContext();
            var dbUser = dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            if (dbUser != null)
            {
                dbUser.IsBlocked = !(dbUser.IsBlocked ?? false);
                dbUser.UpdatedAt = DateTime.Now;
                await dbContext.SaveChangesAsync();
                await LoadUsersAsync();
            }
        }
    }

    private async void AssignTeachers_Click(object? sender, RoutedEventArgs e)
    {
        var course = (sender as Button).Tag as Course;
        if (course == null) return;

        var assignWindow = new Windows.AssignTeachersToCourse(course);
        await assignWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
    }

    private async void DeleteUser_Click(object? sender, RoutedEventArgs e)
    {
        var ask = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Удалить данного пользователя?", ButtonEnum.YesNo);
        if (await ask.ShowAsync() == ButtonResult.Yes)
        {
            var user = (sender as Button).Tag as User;
            var dbContext = new AppDbContext();
            dbContext.Users.Remove(user);
            dbContext.SaveChanges();
            await LoadUsersAsync();
        }
    }
}


