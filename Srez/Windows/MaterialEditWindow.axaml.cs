using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Srez.Data;
using System.Linq;
namespace Srez.Windows
{
    public partial class MaterialEditWindow : Window
    {
        private Material currentMaterial = new Material();
        private bool isEditMode = false;
        public MaterialEditWindow()
        {
            InitializeComponent();
            DataContext = currentMaterial;
            LoadComboBoxes();
        }
        public MaterialEditWindow(Material material)
        {
            InitializeComponent();
            this.currentMaterial = new Material
            {
                Id = material.Id,
                Name = material.Name,
                MaterialTypeId = material.MaterialTypeId,
                MeasurementId = material.MeasurementId,
                StockQuantity = material.StockQuantity,
                PackageQuantity = material.PackageQuantity,
                MinQuantity = material.MinQuantity,
                UnitPrice = material.UnitPrice
            };
            this.isEditMode = true;
            DataContext = this.currentMaterial;
            TitleText.Text = "Редактирование материала";
            Title = "Редактирование материала";
            LoadComboBoxes();
        }
        private async void LoadComboBoxes()
        {
            try
            {
                using var dbContext = new AppDbContext();
                var materialTypes = dbContext.MaterialTypes.ToList();
                CbMaterialType.ItemsSource = materialTypes;
                var measurements = dbContext.MaterialMeasurements.ToList();
                CbMeasurement.ItemsSource = measurements;
                if (isEditMode && currentMaterial.Id > 0)
                {
                    CbMaterialType.SelectedItem = materialTypes.FirstOrDefault(mt => mt.Id == currentMaterial.MaterialTypeId);
                    CbMeasurement.SelectedItem = measurements.FirstOrDefault(m => m.Id == currentMaterial.MeasurementId);
                }
            }
            catch (System.Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Ошибка загрузки данных:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
        private void BtnCancel_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
        private async void BtnSave_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentMaterial.Name))
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка валидации", "Наименование обязательно для заполнения.", ButtonEnum.Ok).ShowAsync();
                return;
            }
            if (CbMaterialType.SelectedItem == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка валидации", "Необходимо выбрать тип материала.", ButtonEnum.Ok).ShowAsync();
                return;
            }
            if (CbMeasurement.SelectedItem == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка валидации", "Необходимо выбрать единицу измерения.", ButtonEnum.Ok).ShowAsync();
                return;
            }
            if (!decimal.TryParse(TbStockQuantity.Text, out decimal stockQuantity) || stockQuantity < 0)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка валидации", "Количество на складе должно быть неотрицательным числом.", ButtonEnum.Ok).ShowAsync();
                TbStockQuantity.Focus();
                return;
            }
            if (!decimal.TryParse(TbPackageQuantity.Text, out decimal packageQuantity) || packageQuantity < 0)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка валидации", "Количество в упаковке должно быть неотрицательным числом.", ButtonEnum.Ok).ShowAsync();
                TbPackageQuantity.Focus();
                return;
            }
            if (!decimal.TryParse(TbMinQuantity.Text, out decimal minQuantity) || minQuantity < 0)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка валидации", "Минимальное количество должно быть неотрицательным числом.", ButtonEnum.Ok).ShowAsync();
                TbMinQuantity.Focus();
                return;
            }
            if (!decimal.TryParse(TbUnitPrice.Text, out decimal unitPrice) || unitPrice < 0)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка валидации", "Цена единицы должна быть неотрицательным числом.", ButtonEnum.Ok).ShowAsync();
                TbUnitPrice.Focus();
                return;
            }
            currentMaterial.StockQuantity = stockQuantity;
            currentMaterial.PackageQuantity = packageQuantity;
            currentMaterial.MinQuantity = minQuantity;
            currentMaterial.UnitPrice = unitPrice;
            try
            {
                var materialType = CbMaterialType.SelectedItem as MaterialType;
                var measurement = CbMeasurement.SelectedItem as MaterialMeasurement;
                using var dbContext = new AppDbContext();
                if (isEditMode && currentMaterial.Id > 0)
                {
                    var existingMaterial = dbContext.Materials
                        .FirstOrDefault(m => m.Id == currentMaterial.Id);
                    if (existingMaterial != null)
                    {
                        existingMaterial.Name = currentMaterial.Name;
                        existingMaterial.MaterialTypeId = materialType!.Id;
                        existingMaterial.MeasurementId = measurement!.Id;
                        existingMaterial.StockQuantity = currentMaterial.StockQuantity;
                        existingMaterial.PackageQuantity = currentMaterial.PackageQuantity;
                        existingMaterial.MinQuantity = currentMaterial.MinQuantity;
                        existingMaterial.UnitPrice = currentMaterial.UnitPrice;
                    }
                }
                else
                {
                    currentMaterial.MaterialTypeId = materialType!.Id;
                    currentMaterial.MeasurementId = measurement!.Id;
                    dbContext.Materials.Add(currentMaterial);
                }
                await dbContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Успех", "Материал успешно сохранен.", ButtonEnum.Ok).ShowAsync();
                Close();
            }
            catch (System.Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Ошибка сохранения материала:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }
    }
}
