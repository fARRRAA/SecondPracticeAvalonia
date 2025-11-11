using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SecondPracticeAvalonia.Models;
using SecondPracticeAvalonia.Pages;

namespace SecondPracticeAvalonia.Windows;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        EmailTextBox.Text = "admin@edutrack.com";
        PasswordTextBox.Text = "fara123";
    }

    private async void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        ErrorText.IsVisible = false;
        var email = EmailTextBox.Text?.Trim() ?? string.Empty;
        var password = PasswordTextBox.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Введите e-mail и пароль.");
            return;
        }

        // Каждый запрос — новый контекст
        using var db = new AppDbContext();
        var user = await Task.Run(() =>
            db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password));

        if (user is null)
        {
            ShowError("Неверный e-mail или пароль.");
            return;
        }

        // Определение роли по связям (без отдельного поля role)
        // Преподаватель — если есть записи в CourseTeachers
        var main = new MainWindow();

        void Logout()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            main.Close();
        }

        // Преподаватель — если есть записи в CourseTeachers
        var isTeacher = await Task.Run(() => db.CourseTeachers.Any(ct => ct.TeacherId == user.Id));
        if (isTeacher)
        {
            var teacherPage = new TeacherPage(user.Id, Logout);
            main.ShowPage(teacherPage, "EduTrack — Преподаватель");
            main.Show();
            Close();
            return;
        }

        // Студент — если есть записи в CourseEnrollments
        var isStudent = await Task.Run(() => db.CourseEnrollments.Any(ce => ce.StudentId == user.Id));
        if (isStudent)
        {
            var studentPage = new StudentPage(user.Id, Logout);
            main.ShowPage(studentPage, "EduTrack — Студент");
            main.Show();
            Close();
            return;
        }

        // Остальные — администраторы (по умолчанию)
        var adminPage = new AdminPage(user.Id, Logout);
        main.ShowPage(adminPage, "EduTrack — Администратор");
        main.Show();
        Close();
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.IsVisible = true;
    }
}


