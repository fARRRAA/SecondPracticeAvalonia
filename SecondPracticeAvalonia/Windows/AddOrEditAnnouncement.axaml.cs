using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AddOrEditAnnouncement : Window
    {
        private Announcement currentAnnouncement = new Announcement();

        public AddOrEditAnnouncement()
        {
            InitializeComponent();
            DataContext = currentAnnouncement;
        }

        public AddOrEditAnnouncement(Announcement announcement)
        {
            InitializeComponent();
            this.currentAnnouncement = announcement;
            DataContext = this.currentAnnouncement;
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentAnnouncement.Title))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Title is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(currentAnnouncement.Content))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Content is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            try
            {
                var dbContext = new AppDbContext();
                if (currentAnnouncement.Id == 0)
                {
                    currentAnnouncement.CreatedAt = DateTime.Now;
                    dbContext.Announcements.Add(currentAnnouncement);
                }
                else
                {
                    currentAnnouncement.UpdatedAt = DateTime.Now;
                    dbContext.Announcements.Update(currentAnnouncement);
                }

                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Announcement saved successfully.", ButtonEnum.Ok).ShowAsync();
                Close();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving announcement:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
    }
}
