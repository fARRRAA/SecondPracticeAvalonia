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
    public partial class BrowseCourses : Window
    {
        private readonly int _studentId;
        private AppDbContext dbContext;

        public BrowseCourses(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            dbContext = new AppDbContext();
            LoadFilters();
            LoadCourses();
            
            CategoryFilter.SelectionChanged += Filter_SelectionChanged;
            LevelFilter.SelectionChanged += Filter_SelectionChanged;
            PriceFilter.TextChanged += Filter_TextChanged;
        }

        private void LoadFilters()
        {
            try
            {
                var categories = new List<object> { new { Id = 0, Name = "All" } };
                categories.AddRange(dbContext.CourseCategories.OrderBy(c => c.Name).ToList());
                CategoryFilter.ItemsSource = categories;

                var levels = new List<object> { new { Id = 0, Name = "All" } };
                levels.AddRange(dbContext.CourseLevels.OrderBy(l => l.Name).ToList());
                LevelFilter.ItemsSource = levels;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading filters:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void Filter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            LoadCourses();
        }

        private void Filter_TextChanged(object? sender, TextChangedEventArgs e)
        {
            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                var query = dbContext.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Level)
                    .Include(c => c.Status)
                    .Where(c => c.Status != null && c.Status.Name.ToLower() == "published");

                // Category filter
                if (CategoryFilter.SelectedItem is CourseCategory selectedCategory)
                {
                    query = query.Where(c => c.CategoryId == selectedCategory.Id);
                }

                // Level filter
                if (LevelFilter.SelectedItem is CourseLevel selectedLevel)
                {
                    query = query.Where(c => c.LevelId == selectedLevel.Id);
                }

                // Price filter
                if (!string.IsNullOrWhiteSpace(PriceFilter.Text) && decimal.TryParse(PriceFilter.Text, out decimal maxPrice))
                {
                    query = query.Where(c => c.Price <= maxPrice);
                }

                var courses = query.OrderBy(c => c.Title).ToList();
                CoursesGrid.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading courses:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void ViewCourse_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Course course)
            {
                var courseContentWindow = new CourseContent(_studentId, course.Id);
                await courseContentWindow.ShowDialog(this);
            }
        }

        private async void EnrollCourse_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Course course)
            {
                try
                {
                    // Check if already enrolled
                    var existingEnrollment = dbContext.CourseEnrollments
                        .FirstOrDefault(ce => ce.StudentId == _studentId && ce.CourseId == course.Id);

                    if (existingEnrollment != null)
                    {
                        await MessageBoxManager.GetMessageBoxStandard("Info", "You are already enrolled in this course.", ButtonEnum.Ok).ShowAsync();
                        return;
                    }

                    var enrollment = new CourseEnrollment
                    {
                        StudentId = _studentId,
                        CourseId = course.Id,
                        EnrolledAt = DateTime.Now,
                        ProgressPercentage = 0
                    };

                    dbContext.CourseEnrollments.Add(enrollment);
                    await dbContext.SaveChangesAsync();
                    await MessageBoxManager.GetMessageBoxStandard("Success", "Successfully enrolled in the course!", ButtonEnum.Ok).ShowAsync();
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Error", $"Error enrolling in course:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                }
            }
        }
    }
}

