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
    public partial class ViewDiscussion : Window
    {
        private readonly int _userId;
        private readonly int _discussionId;
        private readonly bool _isTeacher;
        private AppDbContext dbContext;
        private Discussion? discussion;

        public ViewDiscussion(int userId, int discussionId, bool isTeacher)
        {
            InitializeComponent();
            _userId = userId;
            _discussionId = discussionId;
            _isTeacher = isTeacher;
            dbContext = new AppDbContext();
            LoadDiscussion();
        }

        private void LoadDiscussion()
        {
            try
            {
                discussion = dbContext.Discussions
                    .Include(d => d.Student)
                    .Include(d => d.Lesson)
                    .FirstOrDefault(d => d.Id == _discussionId);

                if (discussion == null)
                {
                    Close();
                    return;
                }

                TitleText.Text = discussion.Title;
                StudentText.Text = $"Student: {discussion.Student?.FirstName} {discussion.Student?.LastName} ({discussion.Student?.Email})";
                QuestionText.Text = discussion.Content ?? "";

                // Show reply button only for teachers
                if (_isTeacher)
                {
                    ReplyTextBox.IsVisible = true;
                    btnReply.IsVisible = true;
                }

                LoadReplies();
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading discussion:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void LoadReplies()
        {
            try
            {
                var replies = dbContext.DiscussionReplies
                    .Where(r => r.DiscussionId == _discussionId)
                    .Include(r => r.User)
                    .OrderBy(r => r.CreatedAt)
                    .ToList();

                RepliesPanel.Children.Clear();

                foreach (var reply in replies)
                {
                    var replyPanel = new StackPanel 
                    { 
                        Spacing = 5, 
                        Margin = new Avalonia.Thickness(0, 0, 0, 15),
                    };

                    var authorText = new TextBlock
                    {
                        Text = $"{reply.User?.FirstName} {reply.User?.LastName} {(reply.IsTeacher == true ? "(Teacher)" : "")}",
                        FontWeight = Avalonia.Media.FontWeight.Bold
                    };
                    replyPanel.Children.Add(authorText);

                    var contentText = new TextBlock
                    {
                        Text = reply.Content,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };
                    replyPanel.Children.Add(contentText);

                    var dateText = new TextBlock
                    {
                        Text = $"Posted: {reply.CreatedAt:dd.MM.yyyy HH:mm}",
                        FontSize = 12,
                        Foreground = Avalonia.Media.Brushes.Gray
                    };
                    replyPanel.Children.Add(dateText);

                    RepliesPanel.Children.Add(replyPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading replies:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnReply_Click(object? sender, RoutedEventArgs e)
        {
            if (!_isTeacher || discussion == null)
                return;

            if (string.IsNullOrWhiteSpace(ReplyTextBox.Text))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Reply content is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            try
            {
                var reply = new DiscussionReply
                {
                    DiscussionId = _discussionId,
                    UserId = _userId,
                    Content = ReplyTextBox.Text,
                    IsTeacher = true,
                    CreatedAt = DateTime.Now
                };

                dbContext.DiscussionReplies.Add(reply);

                // Mark discussion as answered
                discussion.IsAnswered = true;
                discussion.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync();
                ReplyTextBox.Text = "";
                LoadReplies();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Reply posted successfully.", ButtonEnum.Ok).ShowAsync();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error posting reply:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}


