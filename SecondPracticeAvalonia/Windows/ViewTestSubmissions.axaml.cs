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
    public class SubmissionViewModel
    {
        public int Id { get; set; }
        public User? Student { get; set; }
        public string StudentName { get; set; } = "";
        public int? Score { get; set; }
        public int MaxScore { get; set; }
        public AssignmentStatus? Status { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? CheckedAt { get; set; }
        public string? TeacherComment { get; set; }
    }

    public partial class ViewTestSubmissions : Window
    {
        private readonly int _teacherId;
        private AppDbContext dbContext;

        public ViewTestSubmissions(int teacherId)
        {
            InitializeComponent();
            _teacherId = teacherId;
            dbContext = new AppDbContext();
            LoadCourses();
            
            CourseComboBox.SelectionChanged += CourseComboBox_SelectionChanged;
            TestComboBox.SelectionChanged += TestComboBox_SelectionChanged;
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
                LoadTests(selectedCourse.Id);
            }
            else
            {
                TestComboBox.ItemsSource = null;
                SubmissionsGrid.ItemsSource = null;
            }
        }

        private void LoadTests(int courseId)
        {
            try
            {
                // Get all lessons in modules of this course
                var moduleIds = dbContext.CourseModules
                    .Where(m => m.CourseId == courseId)
                    .Select(m => m.Id)
                    .ToList();

                var lessonIds = dbContext.Lessons
                    .Where(l => moduleIds.Contains(l.ModuleId ?? 0))
                    .Select(l => l.Id)
                    .ToList();

                var tests = dbContext.Assignments
                    .Where(a => a.LessonId.HasValue && lessonIds.Contains(a.LessonId.Value))
                    .OrderBy(a => a.Title)
                    .ToList();

                TestComboBox.ItemsSource = tests;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading tests:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void TestComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (TestComboBox.SelectedItem is Assignment selectedTest)
            {
                LoadSubmissions(selectedTest.Id);
            }
            else
            {
                SubmissionsGrid.ItemsSource = null;
            }
        }

        private void LoadSubmissions(int assignmentId)
        {
            try
            {
                // Calculate max score from questions
                var maxScore = dbContext.TestQuestions
                    .Where(q => q.AssignmentId == assignmentId)
                    .Sum(q => (int?)(q.Points ?? 1)) ?? 0;

                var submissions = dbContext.AssignmentSubmissions
                    .Where(s => s.AssignmentId == assignmentId)
                    .Include(s => s.Student)
                    .Include(s => s.Status)
                    .OrderByDescending(s => s.SubmittedAt)
                    .Select(s => new SubmissionViewModel
                    {
                        Id = s.Id,
                        Student = s.Student,
                        StudentName = $"{s.Student!.FirstName} {s.Student!.LastName}",
                        Score = s.Score,
                        MaxScore = maxScore,
                        Status = s.Status,
                        SubmittedAt = s.SubmittedAt,
                        CheckedAt = s.CheckedAt,
                        TeacherComment = s.TeacherComment
                    })
                    .ToList();

                SubmissionsGrid.ItemsSource = submissions;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading submissions:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void ViewDetails_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is SubmissionViewModel submission)
            {
                try
                {
                    var viewDetailsWindow = new ViewTestSubmissionDetails(submission.Id);
                    await viewDetailsWindow.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Error", $"Error opening details:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                }
            }
        }

        private void btnRefresh_Click(object? sender, RoutedEventArgs e)
        {
            if (TestComboBox.SelectedItem is Assignment selectedTest)
            {
                LoadSubmissions(selectedTest.Id);
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}

