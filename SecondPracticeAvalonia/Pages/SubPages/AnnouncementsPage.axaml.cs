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
    public partial class AnnouncementsPage : UserControl
    {
        private readonly int _teacherId;
        private AppDbContext dbContext;
        private readonly ComboBox _courseCb;
        private readonly DataGrid _announcementsDg;

        public AnnouncementsPage(int teacherId)
        {
            InitializeComponent();
            _teacherId = teacherId;
            dbContext = new AppDbContext();

            _courseCb = this.FindControl<ComboBox>("CourseComboBox")!;
            _announcementsDg = this.FindControl<DataGrid>("AnnouncementsGrid")!;

            _courseCb.SelectionChanged += _courseCb_SelectionChanged;
            LoadCourses();
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

                _courseCb.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading courses:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void _courseCb_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_courseCb.SelectedItem is Course selectedCourse)
            {
                LoadAnnouncements(selectedCourse.Id);
            }
            else
            {
                _announcementsDg.ItemsSource = null;
            }
        }

        private void LoadAnnouncements(int courseId)
        {
            try
            {
                var announcements = dbContext.Announcements
                    .Where(a => a.CourseId == courseId)
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new
                    {
                        a.Id,
                        a.Title,
                        a.Content,
                        a.IsPublished,
                        a.CreatedAt
                    })
                    .ToList();

                _announcementsDg.ItemsSource = announcements;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading announcements:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void AddAnnouncement_Click(object? sender, RoutedEventArgs e)
        {
            if (_courseCb.SelectedItem is not Course selectedCourse)
            {
                await MessageBoxManager.GetMessageBoxStandard("Warning", "Please select a course first.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            var announcement = new Announcement
            {
                CourseId = selectedCourse.Id,
                TeacherId = _teacherId,
                CreatedAt = DateTime.Now
            };

            var addWindow = new Windows.AddOrEditAnnouncement(announcement);
            await addWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
            LoadAnnouncements(selectedCourse.Id);
        }

        private async void EditAnnouncement_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                try
                {
                    var announcementData = button.Tag;
                    var idProperty = announcementData.GetType().GetProperty("Id");
                    if (idProperty != null)
                    {
                        var announcementId = (int)idProperty.GetValue(announcementData)!;
                        var announcement = dbContext.Announcements.FirstOrDefault(a => a.Id == announcementId);
                        if (announcement != null)
                        {
                            var editWindow = new Windows.AddOrEditAnnouncement(announcement);
                            await editWindow.ShowDialog((Window)TopLevel.GetTopLevel(this)!);
                            if (_courseCb.SelectedItem is Course selectedCourse)
                            {
                                LoadAnnouncements(selectedCourse.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Error", $"Error editing announcement:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                }
            }
        }

        private async void DeleteAnnouncement_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                try
                {
                    var announcementData = button.Tag;
                    var idProperty = announcementData.GetType().GetProperty("Id");
                    if (idProperty != null)
                    {
                        var announcementId = (int)idProperty.GetValue(announcementData)!;
                        var result = await MessageBoxManager.GetMessageBoxStandard("Confirm", "Are you sure you want to delete this announcement?", ButtonEnum.YesNo).ShowAsync();
                        if (result == ButtonResult.Yes)
                        {
                            var announcement = dbContext.Announcements.FirstOrDefault(a => a.Id == announcementId);
                            if (announcement != null)
                            {
                                dbContext.Announcements.Remove(announcement);
                                await dbContext.SaveChangesAsync();
                                if (_courseCb.SelectedItem is Course selectedCourse)
                                {
                                    LoadAnnouncements(selectedCourse.Id);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Error", $"Error deleting announcement:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                }
            }
        }
    }
}
