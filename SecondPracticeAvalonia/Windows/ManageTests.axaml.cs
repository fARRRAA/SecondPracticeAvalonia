using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;

namespace SecondPracticeAvalonia.Windows
{
    public partial class ManageTests : Window
    {
        private readonly int _teacherId;
        private AppDbContext dbContext;

        public ManageTests(int teacherId)
        {
            InitializeComponent();
            _teacherId = teacherId;
            dbContext = new AppDbContext();
            LoadCourses();
            
            CourseComboBox.SelectionChanged += CourseComboBox_SelectionChanged;
            ModuleComboBox.SelectionChanged += ModuleComboBox_SelectionChanged;
            LessonComboBox.SelectionChanged += LessonComboBox_SelectionChanged;
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

                CourseComboBox.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading courses:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void CourseComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (CourseComboBox.SelectedItem is Course selectedCourse)
            {
                LoadModules(selectedCourse.Id);
            }
            else
            {
                ModuleComboBox.ItemsSource = null;
                LessonComboBox.ItemsSource = null;
                TestsGrid.ItemsSource = null;
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

                ModuleComboBox.ItemsSource = modules;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading modules:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void ModuleComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (ModuleComboBox.SelectedItem is CourseModule selectedModule)
            {
                LoadLessons(selectedModule.Id);
            }
            else
            {
                LessonComboBox.ItemsSource = null;
                TestsGrid.ItemsSource = null;
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

                LessonComboBox.ItemsSource = lessons;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading lessons:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void LessonComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (LessonComboBox.SelectedItem is Lesson selectedLesson)
            {
                LoadTests(selectedLesson.Id);
            }
            else
            {
                TestsGrid.ItemsSource = null;
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

                TestsGrid.ItemsSource = tests;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading tests:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnAddTest_Click(object? sender, RoutedEventArgs e)
        {
            if (LessonComboBox.SelectedItem is not Lesson selectedLesson)
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Please select a lesson first.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            var addTestWindow = new AddOrEditTest(selectedLesson.Id);
            await addTestWindow.ShowDialog(this);
            RefreshTests();
        }

        private async void EditTest_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Assignment assignment)
            {
                var editTestWindow = new AddOrEditTest(assignment);
                await editTestWindow.ShowDialog(this);
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
            if (LessonComboBox.SelectedItem is Lesson selectedLesson)
            {
                LoadTests(selectedLesson.Id);
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}


