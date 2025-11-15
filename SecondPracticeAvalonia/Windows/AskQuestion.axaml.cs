using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AskQuestion : Window
    {
        private readonly int _studentId;
        private readonly int _lessonId;
        private AppDbContext dbContext;

        public AskQuestion(int studentId, int lessonId)
        {
            InitializeComponent();
            _studentId = studentId;
            _lessonId = lessonId;
            dbContext = new AppDbContext();
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }

        private async void btnSubmit_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Title is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(ContentTextBox.Text))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Question content is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            try
            {
                var discussion = new Discussion
                {
                    LessonId = _lessonId,
                    StudentId = _studentId,
                    Title = TitleTextBox.Text,
                    Content = ContentTextBox.Text,
                    IsAnswered = false,
                    CreatedAt = DateTime.Now
                };

                dbContext.Discussions.Add(discussion);
                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Question submitted successfully.", ButtonEnum.Ok).ShowAsync();
                Close();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error submitting question:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
    }
}


