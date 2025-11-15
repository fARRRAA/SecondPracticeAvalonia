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
    public partial class ManageLessons : Window
    {
        private readonly int _teacherId;
        private AppDbContext dbContext;

        public ManageLessons(int teacherId)
        {
            InitializeComponent();
            _teacherId = teacherId;
            dbContext = new AppDbContext();
            LoadCourses();
            
            CourseComboBox.SelectionChanged += CourseComboBox_SelectionChanged;
            ModuleComboBox.SelectionChanged += ModuleComboBox_SelectionChanged;
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
                LessonsGrid.ItemsSource = null;
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
                LessonsGrid.ItemsSource = null;
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

                LessonsGrid.ItemsSource = lessons;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading lessons:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnAddLesson_Click(object? sender, RoutedEventArgs e)
        {
            if (ModuleComboBox.SelectedItem is not CourseModule selectedModule)
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Please select a module first.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            var addLessonWindow = new AddOrEditLesson(selectedModule.Id);
            await addLessonWindow.ShowDialog(this);
            RefreshLessons();
        }

        private async void EditLesson_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Lesson lesson)
            {
                var editLessonWindow = new AddOrEditLesson(lesson);
                await editLessonWindow.ShowDialog(this);
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
            if (ModuleComboBox.SelectedItem is CourseModule selectedModule)
            {
                LoadLessons(selectedModule.Id);
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}


