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
    public partial class ViewLesson : Window
    {
        private readonly int _studentId;
        private readonly int _lessonId;
        private AppDbContext dbContext;
        private Lesson? lesson;
        private LessonProgress? progress;

        public ViewLesson(int studentId, int lessonId)
        {
            InitializeComponent();
            _studentId = studentId;
            _lessonId = lessonId;
            dbContext = new AppDbContext();
            LoadLesson();
        }

        private void LoadLesson()
        {
            try
            {
                lesson = dbContext.Lessons.FirstOrDefault(l => l.Id == _lessonId);
                if (lesson == null)
                {
                    MessageBoxManager.GetMessageBoxStandard("Error", "Lesson not found.", ButtonEnum.Ok).ShowAsync();
                    Close();
                    return;
                }

                LessonTitleText.Text = lesson.Title;
                ContentText.Text = lesson.Content ?? "No content available.";
                VideoUrlText.Text = lesson.VideoUrl ?? "No video available.";

                // Load or create progress
                progress = dbContext.LessonProgresses
                    .FirstOrDefault(lp => lp.StudentId == _studentId && lp.LessonId == _lessonId);

                if (progress == null)
                {
                    progress = new LessonProgress
                    {
                        StudentId = _studentId,
                        LessonId = _lessonId,
                        IsCompleted = false,
                        LastPositionSeconds = 0,
                        CreatedAt = DateTime.Now
                    };
                }

                CompletedCheckBox.IsChecked = progress.IsCompleted ?? false;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading lesson:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (progress == null) return;

                progress.IsCompleted = CompletedCheckBox.IsChecked ?? false;
                if (progress.IsCompleted == true && progress.CompletedAt == null)
                {
                    progress.CompletedAt = DateTime.Now;
                }

                if (progress.Id == 0)
                {
                    dbContext.LessonProgresses.Add(progress);
                }
                else
                {
                    dbContext.LessonProgresses.Update(progress);
                }

                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Progress saved successfully.", ButtonEnum.Ok).ShowAsync();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving progress:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}


