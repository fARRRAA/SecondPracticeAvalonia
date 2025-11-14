using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AddOrEditOption : Window
    {
        private AnswerOption currentOption = new AnswerOption();
        private readonly int? questionId;
        private AppDbContext dbContext=new AppDbContext();

        public AddOrEditOption(int questionId)
        {
            InitializeComponent();
            this.questionId = questionId;
            currentOption.QuestionId = questionId;
            currentOption.OrderIndex = GetNextOrderIndex(questionId);
            currentOption.IsCorrect = false;
            dbContext = new AppDbContext();
            DataContext = currentOption;
        }

        public AddOrEditOption(AnswerOption option)
        {
            InitializeComponent();
            this.currentOption = option;
            dbContext = new AppDbContext();
            DataContext = currentOption;
        }

        private int GetNextOrderIndex(int questionId)
        {
            var maxOrder = dbContext.AnswerOptions
                .Where(o => o.QuestionId == questionId)
                .Select(o => (int?)o.OrderIndex)
                .Max();
            return (maxOrder ?? 0) + 1;
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }

        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentOption.OptionText))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Option text is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            try
            {
                if (currentOption.Id == 0)
                {
                    dbContext.AnswerOptions.Add(currentOption);
                }
                else
                {
                    dbContext.AnswerOptions.Update(currentOption);
                }

                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "Option saved successfully.", ButtonEnum.Ok).ShowAsync();
                Close();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving option:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
    }
}

