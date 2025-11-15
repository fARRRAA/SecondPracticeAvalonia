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
    public class AnswerTag
    {
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
    }

    public partial class TakeTest : Window
    {
        private readonly int _studentId;
        private readonly int _assignmentId;
        private AppDbContext dbContext;
        private Assignment? assignment;
        private List<TestQuestion> questions = new List<TestQuestion>();
        private Dictionary<int, int?> selectedAnswers = new Dictionary<int, int?>();
        private AssignmentSubmission? submission;

        public TakeTest(int studentId, int assignmentId)
        {
            InitializeComponent();
            _studentId = studentId;
            _assignmentId = assignmentId;
            dbContext = new AppDbContext();
            LoadTest();
        }

        private void LoadTest()
        {
            try
            {
                assignment = dbContext.Assignments
                    .Include(a => a.Lesson)
                    .FirstOrDefault(a => a.Id == _assignmentId);

                if (assignment == null)
                {
                    MessageBoxManager.GetMessageBoxStandard("Error", "Test not found.", ButtonEnum.Ok).ShowAsync();
                    Close();
                    return;
                }

                TestTitleText.Text = assignment.Title;
                TestDescriptionText.Text = assignment.Description ?? "";

                // Load questions with answer options
                questions = dbContext.TestQuestions
                    .Include(q => q.AnswerOptions.OrderBy(o => o.OrderIndex))
                    .Where(q => q.AssignmentId == _assignmentId)
                    .OrderBy(q => q.OrderIndex)
                    .ToList();

                // Check if already submitted
                submission = dbContext.AssignmentSubmissions
                    .FirstOrDefault(s => s.StudentId == _studentId && s.AssignmentId == _assignmentId);

                if (submission != null)
                {
                    // Load previous answers
                    var previousAnswers = dbContext.TestAnswers
                        .Where(ta => ta.SubmissionId == submission.Id)
                        .ToList();

                    foreach (var answer in previousAnswers)
                    {
                        if (answer.QuestionId.HasValue && answer.SelectedOptionId.HasValue)
                        {
                            selectedAnswers[answer.QuestionId.Value] = answer.SelectedOptionId.Value;
                        }
                    }

                    btnSubmit.IsEnabled = false;
                    // Calculate max score from questions
                    int maxScoreFromQuestions = questions.Sum(q => q.Points ?? 1);
                    ScoreText.Text = $"Score: {submission.Score ?? 0}/{maxScoreFromQuestions}";
                }

                RenderQuestions();
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading test:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void RenderQuestions()
        {
            QuestionsPanel.Children.Clear();

            foreach (var question in questions)
            {
                var questionPanel = new StackPanel { Spacing = 10, Margin = new Avalonia.Thickness(0, 0, 0, 20) };

                var questionText = new TextBlock
                {
                    Text = $"{question.OrderIndex}. {question.QuestionText} ({question.Points ?? 1} points)",
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                };
                questionPanel.Children.Add(questionText);

                var optionsPanel = new StackPanel { Spacing = 5, Margin = new Avalonia.Thickness(20, 0, 0, 0) };

                foreach (var option in question.AnswerOptions.OrderBy(o => o.OrderIndex))
                {
                    RadioButton radioButton = new RadioButton
                    {
                        Content = option.OptionText,
                        Tag = new AnswerTag { QuestionId = question.Id, OptionId = option.Id },
                        GroupName = $"Question_{question.Id}",
                        IsEnabled = submission == null
                    };

                    if (selectedAnswers.ContainsKey(question.Id) && selectedAnswers[question.Id] == option.Id)
                    {
                        radioButton.IsChecked = true;
                    }

                    radioButton.IsCheckedChanged += RadioButton_IsCheckedChanged;
                    optionsPanel.Children.Add(radioButton);
                }

                questionPanel.Children.Add(optionsPanel);
                QuestionsPanel.Children.Add(questionPanel);
            }
        }

        private void RadioButton_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked == true && radioButton.Tag != null)
            {
                var tag = radioButton.Tag as AnswerTag;
                if (tag != null)
                {
                    selectedAnswers[tag.QuestionId] = tag.OptionId;
                }
            }
        }

        private async void btnSubmit_Click(object? sender, RoutedEventArgs e)
        {
            if (submission != null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Info", "Test already submitted.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            try
            {
                // Create submission
                submission = new AssignmentSubmission
                {
                    StudentId = _studentId,
                    AssignmentId = _assignmentId,
                    SubmittedAt = DateTime.Now,
                    StatusId = GetSubmittedStatusId()
                };

                dbContext.AssignmentSubmissions.Add(submission);
                await dbContext.SaveChangesAsync();

                int totalScore = 0;
                int maxScore = 0;

                // Create test answers and calculate score
                foreach (var question in questions)
                {
                    maxScore += question.Points ?? 1;

                    TestAnswer testAnswer;
                    if (selectedAnswers.ContainsKey(question.Id))
                    {
                        var selectedOptionId = selectedAnswers[question.Id];
                        var selectedOption = question.AnswerOptions.FirstOrDefault(o => o.Id == selectedOptionId);

                        testAnswer = new TestAnswer
                        {
                            SubmissionId = submission.Id,
                            QuestionId = question.Id,
                            SelectedOptionId = selectedOptionId
                        };

                        if (selectedOption != null && selectedOption.IsCorrect == true)
                        {
                            testAnswer.IsCorrect = true;
                            testAnswer.PointsEarned = question.Points ?? 1;
                            totalScore += question.Points ?? 1;
                        }
                        else
                        {
                            testAnswer.IsCorrect = false;
                            testAnswer.PointsEarned = 0;
                        }
                    }
                    else
                    {
                        // No answer selected
                        testAnswer = new TestAnswer
                        {
                            SubmissionId = submission.Id,
                            QuestionId = question.Id,
                            SelectedOptionId = null,
                            IsCorrect = false,
                            PointsEarned = 0
                        };
                    }

                    dbContext.TestAnswers.Add(testAnswer);
                }

                // Update submission with score and status
                submission.Score = totalScore;
                // For automatically graded tests, mark as checked
                var checkedStatus = dbContext.AssignmentStatuses.FirstOrDefault(s => s.Name.ToLower() == "checked");
                if (checkedStatus != null)
                {
                    submission.StatusId = checkedStatus.Id;
                    submission.CheckedAt = DateTime.Now;
                }

                await dbContext.SaveChangesAsync();

                ScoreText.Text = $"Score: {totalScore}/{maxScore}";
                btnSubmit.IsEnabled = false;

                await MessageBoxManager.GetMessageBoxStandard("Success", $"Test submitted!\nYour score: {totalScore}/{maxScore}", ButtonEnum.Ok).ShowAsync();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error submitting test:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private int GetSubmittedStatusId()
        {
            var status = dbContext.AssignmentStatuses.FirstOrDefault(s => s.Name.ToLower() == "submitted");
            return status?.Id ?? 1;
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}

