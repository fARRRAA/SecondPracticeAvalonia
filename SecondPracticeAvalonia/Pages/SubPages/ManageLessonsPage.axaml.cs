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
    public partial class ManageLessonsPage : UserControl
    {
        private readonly int _teacherId;
        private AppDbContext dbContext;
        private readonly ComboBox _coursescb;
        private readonly ComboBox _modulecb;
        private readonly ComboBox _lessonscb;
        private readonly DataGrid _lessonsdg;

        public ManageLessonsPage(int teacherId)
        {
            InitializeComponent();
            _teacherId = teacherId;
            dbContext = new AppDbContext();
            _coursescb = this.FindControl<ComboBox>("CourseComboBox");
            _modulecb = this.FindControl<ComboBox>("ModuleComboBox");
            _lessonsdg = this.FindControl<DataGrid>("LessonsGrid");


            LoadCourses();
            
            _coursescb.SelectionChanged += _coursescb_SelectionChanged;
            _modulecb.SelectionChanged += _modulecb_SelectionChanged;
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

        private void _coursescb_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_coursescb.SelectedItem is Course selectedCourse)
            {
                LoadModules(selectedCourse.Id);
            }
            else
            {
                _modulecb.ItemsSource = null;
                _lessonsdg.ItemsSource = null;
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

        private void _modulecb_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_modulecb.SelectedItem is CourseModule selectedModule)
            {
                LoadLessons(selectedModule.Id);
            }
            else
            {
                _lessonsdg.ItemsSource = null;
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

                _lessonsdg.ItemsSource = lessons;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading lessons:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void AddLesson_Click(object? sender, RoutedEventArgs e)
        {
            if (_modulecb.SelectedItem is not CourseModule selectedModule)
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Please select a module first.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            var addLessonWindow = new Windows.AddOrEditLesson(selectedModule.Id);
            await addLessonWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
            RefreshLessons();
        }

        private async void EditLesson_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Lesson lesson)
            {
                var editLessonWindow = new Windows.AddOrEditLesson(lesson);
                await editLessonWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
                RefreshLessons();
            }
        }

        private async void DeleteLesson_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Lesson lesson)
            {
                var ask = MessageBoxManager.GetMessageBoxStandard("Confirmation", "Are you sure you want to delete this lesson?", ButtonEnum.YesNo);
                if (await ask.ShowAsync() == ButtonResult.Yes)
                {
                    try
                    {
                        var lessonToDelete = dbContext.Lessons.FirstOrDefault(l => l.Id == lesson.Id);
                        if (lessonToDelete != null)
                        {
                            dbContext.Lessons.Remove(lessonToDelete);
                            await dbContext.SaveChangesAsync();
                            RefreshLessons();
                            await MessageBoxManager.GetMessageBoxStandard("Success", "Lesson deleted successfully.", ButtonEnum.Ok).ShowAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        await MessageBoxManager.GetMessageBoxStandard("Error", $"Error deleting lesson:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                    }
                }
            }
        }

        private void RefreshLessons()
        {
            if (_modulecb.SelectedItem is CourseModule selectedModule)
            {
                LoadLessons(selectedModule.Id);
            }
        }
    }
}


