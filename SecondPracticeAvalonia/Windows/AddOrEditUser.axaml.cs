using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SecondPracticeAvalonia.Data;
using System;
using System.Linq;

namespace SecondPracticeAvalonia.Windows
{
    public partial class AddOrEditUser : Window
    {
        private User currentUser = new User();
        private string? password;

        public AddOrEditUser()
        {
            InitializeComponent();
            currentUser.IsActive = true;
            currentUser.IsBlocked = false;
            DataContext = new UserViewModel(currentUser);
            LoadComboBoxes();
        }

        public AddOrEditUser(User user)
        {
            InitializeComponent();
            this.currentUser = user;
            DataContext = new UserViewModel(currentUser);
            LoadComboBoxes();
        }

        private async void LoadComboBoxes()
        {
            var dbContext = new AppDbContext();
            try
            {
                var roles = dbContext.UserRoles.ToList();
                cbRole.ItemsSource = roles;
                
                if (currentUser.Id != 0)
                {
                    cbRole.SelectedItem = roles.FirstOrDefault(r => r.Id == currentUser.RoleId);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error getting data:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as UserViewModel;
            if (viewModel == null) return;

            if (string.IsNullOrWhiteSpace(viewModel.Email))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Email is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.FirstName))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "First Name is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.LastName))
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Last Name is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            if (cbRole.SelectedItem == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Role is required.", ButtonEnum.Ok).ShowAsync();
                return;
            }

            try
            {
                var role = cbRole.SelectedItem as UserRole;
                var dbContext = new AppDbContext();

                // Check if email already exists (for new users or if email changed)
                if (currentUser.Id == 0 || currentUser.Email != viewModel.Email)
                {
                    var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == viewModel.Email);
                    if (existingUser != null && existingUser.Id != currentUser.Id)
                    {
                        await MessageBoxManager.GetMessageBoxStandard("Validation Error", "User with this email already exists.", ButtonEnum.Ok).ShowAsync();
                        return;
                    }
                }

                // Update current user properties
                currentUser.Email = viewModel.Email;
                currentUser.FirstName = viewModel.FirstName;
                currentUser.LastName = viewModel.LastName;
                currentUser.Phone = viewModel.Phone;
                currentUser.RoleId = role!.Id;
                currentUser.IsActive = viewModel.IsActive ?? true;
                currentUser.IsBlocked = viewModel.IsBlocked ?? false;

                // Set password only if provided (for new users or if password changed)
                if (!string.IsNullOrWhiteSpace(viewModel.Password))
                {
                    currentUser.PasswordHash = viewModel.Password;
                }
                else if (currentUser.Id == 0)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Validation Error", "Password is required for new users.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                if (currentUser.Id == 0)
                {
                    currentUser.CreatedAt = DateTime.Now;
                    dbContext.Users.Add(currentUser);
                }
                else
                {
                    currentUser.UpdatedAt = DateTime.Now;
                    dbContext.Users.Update(currentUser);
                }

                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Success", "User saved successfully.", ButtonEnum.Ok).ShowAsync();
                Close();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Error saving user:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
    }

    public class UserViewModel
    {
        private readonly User user;

        public UserViewModel(User user)
        {
            this.user = user;
        }

        public string Email
        {
            get => user.Email;
            set => user.Email = value;
        }

        public string Password { get; set; } = string.Empty;

        public string FirstName
        {
            get => user.FirstName;
            set => user.FirstName = value;
        }

        public string LastName
        {
            get => user.LastName;
            set => user.LastName = value;
        }

        public string? Phone
        {
            get => user.Phone;
            set => user.Phone = value;
        }

        public bool? IsActive
        {
            get => user.IsActive;
            set => user.IsActive = value;
        }

        public bool? IsBlocked
        {
            get => user.IsBlocked;
            set => user.IsBlocked = value;
        }
    }
}

