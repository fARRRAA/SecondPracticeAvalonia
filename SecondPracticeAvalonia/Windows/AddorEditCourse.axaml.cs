using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using SecondPracticeAvalonia.Models;
using SecondPracticeAvalonia.Models.Enums;
using System;
using System.Linq;
using System.Net.NetworkInformation;
namespace SecondPracticeAvalonia.Windows
{
    public partial class AddOrEditCourse : Window
    {
        private Course currentCourse = new Course();
        public AddOrEditCourse()
        {
            InitializeComponent();
            DataContext = currentCourse;
            LoadComboBoxes();
        }
        public AddOrEditCourse(Course course)
        {
            InitializeComponent();
            this.currentCourse = course;
            DataContext = this.currentCourse;
            LoadComboBoxes();
        }
        private async void LoadComboBoxes()
        {
            var dbContext = new AppDbContext();
            try
            {

                var categories =dbContext.CourseCategories.ToList();
                cbCategory.ItemsSource = categories;
                var formats = dbContext.CourseFormats.ToList();
                cbFormat.ItemsSource = formats;
                var statuses = dbContext.CourseStatuses.ToList();
                cbStatus.ItemsSource = statuses;
                var levels = dbContext.CourseLevels.ToList();
                cbLevel.ItemsSource = levels;
                cbCategory.SelectedItem = categories.FirstOrDefault(c => c.Id == currentCourse.CategoryId);
                cbLevel.SelectedItem = levels.FirstOrDefault(l=>l.Id == currentCourse.LevelId);
                cbFormat.SelectedItem = formats.FirstOrDefault(formats => formats.Id == currentCourse.FormatId);
                cbStatus.SelectedItem = statuses.FirstOrDefault(statuses => statuses.Id == currentCourse.StatusId);

            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", "Error getting data:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentCourse.Title))
            {
                await MessageBoxManager.GetMessageBoxStandard( "Validation Error", "Title is required.",ButtonEnum.Ok).ShowAsync();
                return;
            }
            try
            {
                var category = cbCategory.SelectedItem as CourseCategory;
                var format = cbFormat.SelectedItem as CourseFormat;
                var level= cbLevel.SelectedItem as CourseLevel;
                var status = cbStatus.SelectedItem as CourseStatus;
                var dbContext = new AppDbContext();
                if (currentCourse.Id == 0)
                {
                    //currentCourse.Category = category;
                    currentCourse.CategoryId = category.Id;
                    currentCourse.FormatId = format.Id;
                    currentCourse.LevelId = level.Id;
                    currentCourse.StatusId = status.Id;
                    var context =
                    dbContext.Courses.Add(currentCourse);
                }
                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Course saved successfully.", ButtonEnum.Ok).ShowAsync();
                Close();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving course:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
    }
}
