using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AddOrEditLesson : Window
    {
        private Lesson currentLesson = new Lesson();
        private readonly int? moduleId;

        public AddOrEditLesson(int moduleId)
        {
            InitializeComponent();
            this.moduleId = moduleId;
            currentLesson.ModuleId = moduleId;
            currentLesson.OrderIndex = GetNextOrderIndex(moduleId);
            currentLesson.IsPublished = false;
            DataContext = currentLesson;
        }

        public AddOrEditLesson(Lesson lesson)
        {
            InitializeComponent();
            this.currentLesson = lesson;
            DataContext = currentLesson;
        }

        private int GetNextOrderIndex(int moduleId)
        {
            using var db = new AppDbContext();
            var maxOrder = db.Lessons
                .Where(l => l.ModuleId == moduleId)
                .Select(l => (int?)l.OrderIndex)
                .Max();
            return (maxOrder ?? 0) + 1;
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentLesson.Title))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Title is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            try
            {
                using var dbContext = new AppDbContext();

                if (currentLesson.Id == 0)
                {
                    currentLesson.CreatedAt = DateTime.Now;
                    dbContext.Lessons.Add(currentLesson);
                }
                else
                {
                    currentLesson.UpdatedAt = DateTime.Now;
                    dbContext.Lessons.Update(currentLesson);
                }

                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Lesson saved successfully.", ButtonEnum.Ok).ShowAsync();
                Close();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving lesson:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
    }
}

