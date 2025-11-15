using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;

namespace SecondPracticeAvalonia.Pages.SubPages
{
    public partial class ManageTestsPage : UserControl
    {
        private readonly int _teacherId;
        private AppDbContext dbContext;
        private readonly ComboBox _coursescb;
        private readonly ComboBox _modulecb;
        private readonly ComboBox _lessonscb;
        private readonly DataGrid _testdg;
        public ManageTestsPage(int teacherId)
        {
            InitializeComponent();
            _teacherId = teacherId;
            dbContext = new AppDbContext();
            _coursescb = this.FindControl<ComboBox>("CourseComboBox");
            _modulecb = this.FindControl<ComboBox>("ModuleComboBox");
            _lessonscb = this.FindControl<ComboBox>("LessonComboBox");
            _testdg = this.FindControl<DataGrid>("TestsGrid");
            LoadCourses();

            _coursescb.SelectionChanged += CourseComboBox_SelectionChanged;
            _modulecb.SelectionChanged += ModuleComboBox_SelectionChanged;
            _lessonscb.SelectionChanged += LessonComboBox_SelectionChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadCourses()
        {
            try
            {
                var courses = dbContext.CourseTeachers
                    .Where(ct => ct.TeacherId == _teacherId)
                    .Include(ct => ct.Course)
                    .Select(ct => ct.Course)
                    .OrderBy(c => c.Title)
                    .ToList();

                _coursescb.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading courses:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void CourseComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_coursescb.SelectedItem is Course selectedCourse)
            {
                LoadModules(selectedCourse.Id);
            }
            else
            {
                _modulecb.ItemsSource = null;
                _lessonscb.ItemsSource = null;
                _testdg.ItemsSource = null;
            }
        }

        private void LoadModules(int courseId)
        {
            try
            {
                var modules = dbContext.CourseModules
                    .Where(m => m.CourseId == courseId)
                    .OrderBy(m => m.OrderIndex)
                    .ToList();

                _modulecb.ItemsSource = modules;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading modules:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void ModuleComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_modulecb.SelectedItem is CourseModule selectedModule)
            {
                LoadLessons(selectedModule.Id);
            }
            else
            {
                _lessonscb.ItemsSource = null;
                _testdg.ItemsSource = null;
            }
        }

        private void LoadLessons(int moduleId)
        {
            try
            {
                var lessons = dbContext.Lessons
                    .Where(l => l.ModuleId == moduleId)
                    .OrderBy(l => l.OrderIndex)
                    .ToList();

                _lessonscb.ItemsSource = lessons;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading lessons:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void LessonComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_lessonscb.SelectedItem is Lesson selectedLesson)
            {
                LoadTests(selectedLesson.Id);
            }
            else
            {
                _testdg.ItemsSource = null;
            }
        }

        private void LoadTests(int lessonId)
        {
            try
            {
                var tests = dbContext.Assignments
                    .Where(a => a.LessonId == lessonId)
                    .Include(a => a.Type)
                    .OrderBy(a => a.CreatedAt)
                    .ToList();

                _testdg.ItemsSource = tests;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading tests:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void AddTest_Click(object? sender, RoutedEventArgs e)
        {
            if (_lessonscb.SelectedItem is not Lesson selectedLesson)
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Please select a lesson first.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            var addTestWindow = new Windows.AddOrEditTest(selectedLesson.Id);
            await addTestWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
            RefreshTests();
        }

        private async void EditTest_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Assignment assignment)
            {
                var editTestWindow = new Windows.AddOrEditTest(assignment);
                await editTestWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
                RefreshTests();
            }
        }

        private async void DeleteTest_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Assignment assignment)
            {
                var ask = MessageBoxManager.GetMessageBoxStandard("Confirmation", "Are you sure you want to delete this test?", ButtonEnum.YesNo);
                if (await ask.ShowAsync() == ButtonResult.Yes)
                {
                    try
                    {
                        var testToDelete = dbContext.Assignments
                            .Include(a => a.TestQuestions)
                            .ThenInclude(q => q.AnswerOptions)
                            .FirstOrDefault(a => a.Id == assignment.Id);
                        
                        if (testToDelete != null)
                        {
                            dbContext.Assignments.Remove(testToDelete);
                            await dbContext.SaveChangesAsync();
                            RefreshTests();
                            await MessageBoxManager.GetMessageBoxStandard("Success", "Test deleted successfully.", ButtonEnum.Ok).ShowAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        await MessageBoxManager.GetMessageBoxStandard("Error", $"Error deleting test:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                    }
                }
            }
        }

        private void RefreshTests()
        {
            if (_lessonscb.SelectedItem is Lesson selectedLesson)
            {
                LoadTests(selectedLesson.Id);
            }
        }
    }
}


