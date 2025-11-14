using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecondPracticeAvalonia.Data;
using SecondPracticeAvalonia.Pages;
using System.Linq;
using System.Threading.Tasks;

namespace SecondPracticeAvalonia;

public partial class LoginPage : UserControl
{
    public LoginPage()
    {

        InitializeComponent();
        EmailTextBox.Text = "teacher2@edutrack.com";
        PasswordTextBox.Text = "fara123";
    }

    private async void LoginButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ErrorText.IsVisible = false;
        var email = EmailTextBox.Text?.Trim() ?? string.Empty;
        var password = PasswordTextBox.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("¬ведите e-mail и пароль.");
            return;
        }

        using var db = new AppDbContext();
        var user = await Task.Run(() =>
            db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password));

        if (user is null)
        {
            ShowError("Ќеверный e-mail или пароль.");
            return;
        }
        var parentWindow = (MainWindow)TopLevel.GetTopLevel(this)!;

        void Logout()
        {
            parentWindow.Navigate(new LoginPage(), "EduTrack Ч ¬ход");

        }

        var isTeacher = await Task.Run(() => db.CourseTeachers.Any(ct => ct.TeacherId == user.Id));
        if (isTeacher)
        {
            parentWindow.Navigate(new TeacherPage(user.Id, Logout),"EduTrack Ч ѕреподаватель");

            return;
        }

        var isStudent = await Task.Run(() => db.CourseEnrollments.Any(ce => ce.StudentId == user.Id));
        if (isStudent)
        {
            parentWindow.Navigate(new StudentPage(user.Id, Logout), "EduTrack Ч —тудент");
            return;
        }

        parentWindow.Navigate(new AdminPage(user.Id, Logout), "EduTrack Ч јдминистратор");
    }
    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.IsVisible = true;
    }
}