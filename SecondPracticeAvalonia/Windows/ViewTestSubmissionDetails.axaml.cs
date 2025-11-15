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
    public partial class ViewTestSubmissionDetails : Window
    {
        private readonly int _submissionId;
        private AppDbContext dbContext;

        public ViewTestSubmissionDetails(int submissionId)
        {
            InitializeComponent();
            _submissionId = submissionId;
            dbContext = new AppDbContext();
            LoadSubmissionDetails();
        }

        private void LoadSubmissionDetails()
        {
            try
            {
                var submission = dbContext.AssignmentSubmissions
                    .Include(s => s.Student)
                    .Include(s => s.Assignment)
                    .FirstOrDefault(s => s.Id == _submissionId);

                if (submission == null || submission.Student == null || submission.Assignment == null)
                {
                    Close();
                    return;
                }

                StudentNameText.Text = $"{submission.Student.FirstName} {submission.Student.LastName} ({submission.Student.Email})";
                ScoreText.Text = $"Score: {submission.Score ?? 0}/{submission.Assignment.MaxScore ?? 0}";

                // Load test answers with questions and options
                var testAnswers = dbContext.TestAnswers
                    .Where(ta => ta.SubmissionId == _submissionId)
                    .Include(ta => ta.Question)
                    .ThenInclude(q => q!.AnswerOptions)
                    .Include(ta => ta.SelectedOption)
                    .OrderBy(ta => ta.Question!.OrderIndex)
                    .ToList();

                AnswersPanel.Children.Clear();

                foreach (var testAnswer in testAnswers)
                {
                    if (testAnswer.Question == null) continue;

                    var answerPanel = new StackPanel { Spacing = 10, Margin = new Avalonia.Thickness(0, 0, 0, 20) };

                    var questionText = new TextBlock
                    {
                        Text = $"{testAnswer.Question.OrderIndex}. {testAnswer.Question.QuestionText} ({testAnswer.Question.Points ?? 1} points)",
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };
                    answerPanel.Children.Add(questionText);

                    var optionsPanel = new StackPanel { Spacing = 5, Margin = new Avalonia.Thickness(20, 0, 0, 0) };

                    foreach (var option in testAnswer.Question.AnswerOptions.OrderBy(o => o.OrderIndex))
                    {
                        var optionText = new TextBlock
                        {
                            Text = option.OptionText,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        };

                        if (testAnswer.SelectedOptionId == option.Id)
                        {
                            optionText.FontWeight = Avalonia.Media.FontWeight.Bold;
                            if (option.IsCorrect == true)
                            {
                                optionText.Foreground = Avalonia.Media.Brushes.Green;
                                optionText.Text = "✓ " + optionText.Text + " (Your answer - Correct)";
                            }
                            else
                            {
                                optionText.Foreground = Avalonia.Media.Brushes.Red;
                                optionText.Text = "✗ " + optionText.Text + " (Your answer - Incorrect)";
                            }
                        }
                        else if (option.IsCorrect == true)
                        {
                            optionText.Foreground = Avalonia.Media.Brushes.Blue;
                            optionText.Text = "→ " + optionText.Text + " (Correct answer)";
                        }

                        optionsPanel.Children.Add(optionText);
                    }

                    var pointsText = new TextBlock
                    {
                        Text = $"Points earned: {testAnswer.PointsEarned ?? 0}/{testAnswer.Question.Points ?? 1}",
                        Margin = new Avalonia.Thickness(20, 5, 0, 0),
                        FontStyle = Avalonia.Media.FontStyle.Italic
                    };
                    answerPanel.Children.Add(optionsPanel);
                    answerPanel.Children.Add(pointsText);

                    AnswersPanel.Children.Add(answerPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading submission details:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}


