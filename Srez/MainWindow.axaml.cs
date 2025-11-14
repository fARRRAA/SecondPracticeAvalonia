using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Srez.Data;
using Srez.ExtendedClasses;
using Srez.Windows;
using System.Linq;

namespace Srez
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            await LoadMaterialsAsync();
        }

        private async System.Threading.Tasks.Task LoadMaterialsAsync()
        {
            try
            {


                using var dbContext = new AppDbContext();
                var materials = dbContext.Materials
                        .Include(m => m.MaterialType)
                        .Include(m => m.Measurement)
                        .OrderBy(x => x.Id)
                        .ToList();

                var materialViewModels = materials.Select(m => new MaterialExtended(m)).ToList();

                MaterialsList.ItemsSource = materialViewModels;
                MaterialsList.InvalidateVisual();
                MaterialsList.UpdateLayout();
            }
            catch (System.Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Ошибка загрузки материалов:\n{ex.Message}\n\nДетали:\n{ex.StackTrace}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void LoadMaterials()
        {
            _ = LoadMaterialsAsync();
        }

        private async void BtnAddMaterial_Click(object? sender, RoutedEventArgs e)
        {
            var editWindow = new MaterialEditWindow();
            await editWindow.ShowDialog(this);
            await LoadMaterialsAsync();
        }

        private async void BtnEdit_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MaterialExtended materialViewModel)
            {
                using var dbContext = new AppDbContext();
                var materialToEdit = dbContext.Materials
                    .Include(m => m.MaterialType)
                    .Include(m => m.Measurement)
                    .FirstOrDefault(m => m.Id == materialViewModel.Id);

                if (materialToEdit != null)
                {
                    var editWindow = new MaterialEditWindow(materialToEdit);
                    await editWindow.ShowDialog(this);
                    await LoadMaterialsAsync();
                }
            }
        }

        private async void BtnSuppliers_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MaterialExtended materialViewModel)
            {
                using var dbContext = new AppDbContext();
                var materialToView = dbContext.Materials
                    .Include(m => m.MaterialType)
                    .Include(m => m.Measurement)
                    .FirstOrDefault(m => m.Id == materialViewModel.Id);

                if (materialToView != null)
                {
                    var suppliersWindow = new SuppliersWindow(materialToView);
                    await suppliersWindow.ShowDialog(this);
                }
            }
        }

        private void MaterialsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (MaterialsList != null)
            {
                MaterialsList.SelectedItem = null;
            }
        }
        private async void MaterialsList_DoubleTapped(object? sender, TappedEventArgs e)
        {
            if (sender is Button button && button.Tag is MaterialExtended materialViewModel)
            {
                using var dbContext = new AppDbContext();
                var materialToEdit = dbContext.Materials
                    .Include(m => m.MaterialType)
                    .Include(m => m.Measurement)
                    .FirstOrDefault(m => m.Id == materialViewModel.Id);

                if (materialToEdit != null)
                {
                    var editWindow = new MaterialEditWindow(materialToEdit);
                    await editWindow.ShowDialog(this);
                    await LoadMaterialsAsync();
                }
            }
        }
    }
}