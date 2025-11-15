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
    public partial class StudentDiscussionsPage : UserControl
    {
        private readonly int _studentId;
        private AppDbContext dbContext;
        private readonly ComboBox _coursescb;
        private readonly ComboBox _lessonscb;
        private readonly DataGrid _discdg;
        public StudentDiscussionsPage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            dbContext = new AppDbContext();
            _coursescb = this.FindControl<ComboBox>("CourseComboBox");
            _lessonscb = this.FindControl<ComboBox>("LessonComboBox");
            _discdg = this.FindControl<DataGrid>("DiscussionsGrid");
            LoadCourses();
            
            _coursescb.SelectionChanged += _coursescb_SelectionChanged;
            _lessonscb.SelectionChanged += _lessonscb_SelectionChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadCourses()
        {
            try
            {
                var courses = dbContext.CourseEnrollments
                    .Where(ce => ce.StudentId == _studentId)
                    .Include(ce => ce.Course)
                    .Select(ce => ce.Course)
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
                LoadLessons(selectedCourse.Id);
            }
            else
            {
                _lessonscb.ItemsSource = null;
                _discdg.ItemsSource = null;
            }
        }

        private void LoadLessons(int courseId)
        {
            try
            {
                var moduleIds = dbContext.CourseModules
                    .Where(m => m.CourseId == courseId)
                    .Select(m => m.Id)
                    .ToList();

                var lessons = dbContext.Lessons
                    .Where(l => moduleIds.Contains(l.ModuleId ?? 0))
                    .OrderBy(l => l.OrderIndex)
                    .ToList();

                _lessonscb.ItemsSource = lessons;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading lessons:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void _lessonscb_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_lessonscb.SelectedItem is Lesson selectedLesson)
            {
                LoadDiscussions(selectedLesson.Id);
            }
            else
            {
                _discdg.ItemsSource = null;
            }
        }

        private void LoadDiscussions(int lessonId)
        {
            try
            {
                var discussions = dbContext.Discussions
                    .Where(d => d.LessonId == lessonId && d.StudentId == _studentId)
                    .OrderByDescending(d => d.CreatedAt)
                    .ToList();

                _discdg.ItemsSource = discussions;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading discussions:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void AskQuestion_Click(object? sender, RoutedEventArgs e)
        {
            if (_lessonscb.SelectedItem is not Lesson selectedLesson)
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Please select a lesson first.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            var askQuestionWindow = new Windows.AskQuestion(_studentId, selectedLesson.Id);
            await askQuestionWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
            RefreshDiscussions();
        }

        private async void ViewDiscussion_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Discussion discussion)
            {
                var viewDiscussionWindow = new Windows.ViewDiscussion(_studentId, discussion.Id, isTeacher: false);
                await viewDiscussionWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
                RefreshDiscussions();
            }
        }

        private void RefreshDiscussions()
        {
            if (_lessonscb.SelectedItem is Lesson selectedLesson)
            {
                LoadDiscussions(selectedLesson.Id);
            }
        }
    }
}


