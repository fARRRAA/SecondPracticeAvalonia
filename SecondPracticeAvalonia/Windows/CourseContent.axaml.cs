using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecondPracticeAvalonia.Windows
{
    public class TreeViewItemData
    {
        public string Type { get; set; } = "";
        public object? Data { get; set; }
    }

    public partial class CourseContent : Window
    {
        private readonly int _studentId;
        private readonly int _courseId;
        private AppDbContext dbContext;
        private Lesson? selectedLesson;
        private Assignment? selectedTest;

        public CourseContent(int studentId, int courseId)
        {
            InitializeComponent();
            _studentId = studentId;
            _courseId = courseId;
            dbContext = new AppDbContext();
            LoadCourseContent();
            
            ContentTreeView.SelectionChanged += ContentTreeView_SelectionChanged;
        }

        private void LoadCourseContent()
        {
            try
            {
                var course = dbContext.Courses
                    .Include(c => c.CourseModules)
                    .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.Assignments)
                    .FirstOrDefault(c => c.Id == _courseId);

                if (course == null) return;

                CourseTitleText.Text = course.Title;

                var treeItems = new List<TreeViewItem>();

                foreach (var module in course.CourseModules.OrderBy(m => m.OrderIndex))
                {
                    var moduleItem = new TreeViewItem { Header = $"Module: {module.Title}" };

                    foreach (var lesson in module.Lessons.OrderBy(l => l.OrderIndex))
                    {
                        var lessonItem = new TreeViewItem 
                        { 
                            Header = $"Lesson: {lesson.Title}",
                            Tag = new TreeViewItemData { Type = "Lesson", Data = lesson }
                        };

                        // Add assignments/tests for this lesson
                        var assignments = dbContext.Assignments
                            .Where(a => a.LessonId == lesson.Id)
                            .ToList();

                        foreach (var assignment in assignments)
                        {
                            var testItem = new TreeViewItem
                            {
                                Header = $"Test: {assignment.Title}",
                                Tag = new TreeViewItemData { Type = "Test", Data = assignment }
                            };
                            lessonItem.Items.Add(testItem);
                        }

                        moduleItem.Items.Add(lessonItem);
                    }

                    treeItems.Add(moduleItem);
                }

                ContentTreeView.ItemsSource = treeItems;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading course content:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void ContentTreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            selectedLesson = null;
            selectedTest = null;
            btnOpenLesson.IsEnabled = false;
            btnOpenTest.IsEnabled = false;

            if (ContentTreeView.SelectedItem is TreeViewItem selectedItem && selectedItem.Tag is TreeViewItemData tag)
            {
                if (tag.Type == "Lesson" && tag.Data is Lesson lesson)
                {
                    selectedLesson = lesson;
                    btnOpenLesson.IsEnabled = true;
                }
                else if (tag.Type == "Test" && tag.Data is Assignment assignment)
                {
                    selectedTest = assignment;
                    btnOpenTest.IsEnabled = true;
                }
            }
        }

        private async void btnOpenLesson_Click(object? sender, RoutedEventArgs e)
        {
            if (selectedLesson != null)
            {
                var viewLessonWindow = new ViewLesson(_studentId, selectedLesson.Id);
                await viewLessonWindow.ShowDialog(this);
            }
        }

        private async void btnOpenTest_Click(object? sender, RoutedEventArgs e)
        {
            if (selectedTest != null)
            {
                var takeTestWindow = new TakeTest(_studentId, selectedTest.Id);
                await takeTestWindow.ShowDialog(this);
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}

