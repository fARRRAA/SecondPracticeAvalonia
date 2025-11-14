using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AddOrEditTest : Window
    {
        private Assignment currentTest = new Assignment();
        private readonly int? lessonId;
        private AppDbContext dbContext;
        private TestViewModel viewModel;

        public AddOrEditTest(int lessonId)
        {
            InitializeComponent();
            this.lessonId = lessonId;
            currentTest.LessonId = lessonId;
            dbContext = new AppDbContext();
            LoadTestType();
            viewModel = new TestViewModel(currentTest);
            DataContext = viewModel;
            LoadQuestions();
        }

        public AddOrEditTest(Assignment assignment)
        {
            InitializeComponent();
            this.currentTest = assignment;
            dbContext = new AppDbContext();
            LoadTestType();
            viewModel = new TestViewModel(currentTest);
            DataContext = viewModel;
            LoadQuestions();
        }

        private void LoadTestType()
        {
            // Set default type to "test" if not set
            if (currentTest.TypeId == 0)
            {
                var testType = dbContext.AssignmentTypes.FirstOrDefault(t => t.Name.ToLower() == "test");
                if (testType != null)
                {
                    currentTest.TypeId = testType.Id;
                }
            }
        }

        private void LoadQuestions()
        {
            if (currentTest.Id == 0)
            {
                QuestionsGrid.ItemsSource = null;
                return;
            }

            try
            {
                // Reload test from database to get the ID if it was just created
                if (currentTest.Id > 0)
                {
                    var questions = dbContext.TestQuestions
                        .Where(q => q.AssignmentId == currentTest.Id)
                        .OrderBy(q => q.OrderIndex)
                        .ToList();

                    QuestionsGrid.ItemsSource = questions;
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Error", $"Error loading questions:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void btnAddQuestion_Click(object? sender, RoutedEventArgs e)
        {
            // First save the test if it's new
            if (currentTest.Id == 0)
            {
                if (!await SaveTest())
                    return;
            }

            var addQuestionWindow = new AddOrEditQuestion(currentTest.Id);
            await addQuestionWindow.ShowDialog(this);
            LoadQuestions();
        }

        private async void EditQuestion_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TestQuestion question)
            {
                var editQuestionWindow = new AddOrEditQuestion(question);
                await editQuestionWindow.ShowDialog(this);
                LoadQuestions();
            }
        }

        private async void DeleteQuestion_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TestQuestion question)
            {
                var ask = MessageBoxManager.GetMessageBoxStandard("Confirmation", "Are you sure you want to delete this question?", ButtonEnum.YesNo);
                if (await ask.ShowAsync() == ButtonResult.Yes)
                {
                    try
                    {
                        var questionToDelete = dbContext.TestQuestions
                            .Include(q => q.AnswerOptions)
                            .FirstOrDefault(q => q.Id == question.Id);
                        
                        if (questionToDelete != null)
                        {
                            dbContext.TestQuestions.Remove(questionToDelete);
                            await dbContext.SaveChangesAsync();
                            LoadQuestions();
                            await MessageBoxManager.GetMessageBoxStandard("Success", "Question deleted successfully.", ButtonEnum.Ok).ShowAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        await MessageBoxManager.GetMessageBoxStandard("Error", $"Error deleting question:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                    }
                }
            }
        }

        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            await SaveTest();
        }

        private async Task<bool> SaveTest()
        {
            // Update currentTest from viewModel
            currentTest.Title = viewModel.Title;
            currentTest.Description = viewModel.Description;
            currentTest.MaxScore = viewModel.MaxScore;
            currentTest.IsRequired = viewModel.IsRequired;
            
            // Convert DateTimeOffset to DateTime
            if (viewModel.Deadline.HasValue)
            {
                currentTest.Deadline = viewModel.Deadline.Value.DateTime;
            }
            else
            {
                currentTest.Deadline = null;
            }

            if (string.IsNullOrWhiteSpace(currentTest.Title))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Title is required.", ButtonEnum.Ok).ShowAsync();
                return false;
            }

            if (string.IsNullOrWhiteSpace(currentTest.Description))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Description is required.", ButtonEnum.Ok).ShowAsync();
                return false;
            }

            try
            {
                if (currentTest.Id == 0)
                {
                    currentTest.CreatedAt = DateTime.Now;
                    dbContext.Assignments.Add(currentTest);
                }
                else
                {
                    currentTest.UpdatedAt = DateTime.Now;
                    dbContext.Assignments.Update(currentTest);
                }

                await dbContext.SaveChangesAsync();
                
                await MessageBoxManager.GetMessageBoxStandard("Success", "Test saved successfully.", ButtonEnum.Ok).ShowAsync();
                LoadQuestions();
                return true;
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving test:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
                return false;
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
            Close();
        }
    }

    public class TestViewModel
    {
        private readonly Assignment assignment;

        public TestViewModel(Assignment assignment)
        {
            this.assignment = assignment;
        }

        public string Title
        {
            get => assignment.Title;
            set => assignment.Title = value;
        }

        public string Description
        {
            get => assignment.Description;
            set => assignment.Description = value;
        }

        public int? MaxScore
        {
            get => assignment.MaxScore;
            set => assignment.MaxScore = value;
        }

        public bool? IsRequired
        {
            get => assignment.IsRequired;
            set => assignment.IsRequired = value;
        }

        public DateTimeOffset? Deadline
        {
            get => assignment.Deadline.HasValue ? new DateTimeOffset(assignment.Deadline.Value) : null;
            set => assignment.Deadline = value?.DateTime;
        }
    }
}

