using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;

namespace SecondPracticeAvalonia.Pages.SubPages
{
    public partial class StudentAnnouncementsPage : UserControl
    {
        private readonly int _studentId;
        private AppDbContext dbContext;
        private readonly DataGrid _announcementsDg;

        public StudentAnnouncementsPage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            dbContext = new AppDbContext();

            _announcementsDg = this.FindControl<DataGrid>("AnnouncementsGrid")!;

            LoadAnnouncements();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadAnnouncements()
        {
            try
            {
                var announcements = dbContext.Announcements
                    .Where(a => a.IsPublished == true &&
                                dbContext.CourseEnrollments
                                    .Any(ce => ce.StudentId == _studentId && ce.CourseId == a.CourseId))
                    .Include(a => a.Course)
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new
                    {
                        a.Id,
                        a.Title,
                        a.Content,
                        CourseName = a.Course!.Title,
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
    }
}
