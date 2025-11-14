using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AddOrEditQuestion : Window
    {
        private TestQuestion currentQuestion = new TestQuestion();
        private readonly int? assignmentId;
        private AppDbContext dbContext;

        public AddOrEditQuestion(int assignmentId)
        {
            InitializeComponent();
            dbContext = new AppDbContext();

            this.assignmentId = assignmentId;
            currentQuestion.AssignmentId = assignmentId;
            currentQuestion.QuestionType = "multiple_choice";
            currentQuestion.OrderIndex = GetNextOrderIndex(assignmentId);
            currentQuestion.Points = 1;
            dbContext = new AppDbContext();
            DataContext = currentQuestion;
            LoadOptions();
        }

        public AddOrEditQuestion(TestQuestion question)
        {
            InitializeComponent();
            this.currentQuestion = question;
            dbContext = new AppDbContext();
            DataContext = currentQuestion;
            LoadQuestionType();
            LoadOptions();
        }

        private int GetNextOrderIndex(int assignmentId)
        {
            var maxOrder = dbContext.TestQuestions
                .Where(q => q.AssignmentId == assignmentId)
                .Select(q => (int?)q.OrderIndex)
                .Max();
            return (maxOrder ?? 0) + 1;
        }

        private void LoadQuestionType()
        {
            if (!string.IsNullOrEmpty(currentQuestion.QuestionType))
            {
                // Set selected item based on question type
                if (currentQuestion.QuestionType == "multiple_choice")
                    QuestionTypeComboBox.SelectedIndex = 0;
                else if (currentQuestion.QuestionType == "single_choice")
                    QuestionTypeComboBox.SelectedIndex = 1;
                else if (currentQuestion.QuestionType == "text")
                    QuestionTypeComboBox.SelectedIndex = 2;
            }
        }

        private void LoadOptions()
        {
            if (currentQuestion.Id == 0)
            {
                OptionsGrid.ItemsSource = null;
                return;
            }

            try
            {
                var options = dbContext.AnswerOptions
                    .Where(o => o.QuestionId == currentQuestion.Id)
                    .OrderBy(o => o.OrderIndex)
                    .ToList();

                OptionsGrid.ItemsSource = options;
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading options:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnAddOption_Click(object? sender, RoutedEventArgs e)
        {
            // First save the question if it's new
            if (currentQuestion.Id == 0)
            {
                if (!await SaveQuestion())
                    return;
            }

            var addOptionWindow = new AddOrEditOption(currentQuestion.Id);
            await addOptionWindow.ShowDialog(this);
            LoadOptions();
        }

        private async void EditOption_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is AnswerOption option)
            {
                var editOptionWindow = new AddOrEditOption(option);
                await editOptionWindow.ShowDialog(this);
                LoadOptions();
            }
        }

        private async void DeleteOption_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is AnswerOption option)
            {
                var ask = MessageBoxManager.GetMessageBoxStandard("Confirmation", "Are you sure you want to delete this option?", ButtonEnum.YesNo);
                if (await ask.ShowAsync() == ButtonResult.Yes)
                {
                    try
                    {
                        var optionToDelete = dbContext.AnswerOptions.FirstOrDefault(o => o.Id == option.Id);
                        if (optionToDelete != null)
                        {
                            dbContext.AnswerOptions.Remove(optionToDelete);
                            await dbContext.SaveChangesAsync();
                            LoadOptions();
                            await MessageBoxManager.GetMessageBoxStandard("Success", "Option deleted successfully.", ButtonEnum.Ok).ShowAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        await MessageBoxManager.GetMessageBoxStandard("Error", $"Error deleting option:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                    }
                }
            }
        }

        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            await SaveQuestion();
        }

        private async Task<bool> SaveQuestion()
        {
            if (string.IsNullOrWhiteSpace(currentQuestion.QuestionText))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Question text is required.", ButtonEnum.Ok).ShowAsync();
                return false;
            }

            if (QuestionTypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                currentQuestion.QuestionType = selectedType.Content?.ToString() ?? "multiple_choice";
            }

            try
            {
                if (currentQuestion.Id == 0)
                {
                    currentQuestion.CreatedAt = DateTime.Now;
                    dbContext.TestQuestions.Add(currentQuestion);
                }
                else
                {
                    dbContext.TestQuestions.Update(currentQuestion);
                }

                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Question saved successfully.", ButtonEnum.Ok).ShowAsync();
                LoadOptions();
                return true;
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving question:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                return false;
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }
}

