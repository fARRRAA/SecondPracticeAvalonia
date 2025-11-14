using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Srez.Data;
using Srez.Services;
using System.Globalization;
using System.Linq;

namespace Srez.Windows
{
    public partial class SuppliersWindow : Window
    {
        private Material material;
        private System.Collections.Generic.List<ProductType> productTypes = new();

        public SuppliersWindow(Material material)
        {
            InitializeComponent();
            this.material = material;
            TitleText.Text = $"Поставщики материала: {material.Name}";
            Title = $"Поставщики материала: {material.Name}";
            Loaded += SuppliersWindow_Loaded;
        }

        private async void SuppliersWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            await LoadSuppliersAsync();
        }

        private async System.Threading.Tasks.Task LoadSuppliersAsync()
        {
            try
            {
                if (SuppliersList == null)
                {
                    System.Diagnostics.Debug.WriteLine("SuppliersList is null!");
                    return;
                }


                using var dbContext = new AppDbContext();

                productTypes = await System.Threading.Tasks.Task.Run(() => dbContext.ProductTypes.ToList());
                ProductTypeCombo.ItemsSource = productTypes;
                if (productTypes.Count > 0)
                {
                    ProductTypeCombo.SelectedIndex = 0;
                }

                var supplierMaterials = await System.Threading.Tasks.Task.Run(() =>
                {
                    return dbContext.SupplierMaterials
                        .Include(sm => sm.Supplier)
                        .Where(sm => sm.MaterialId == material.Id)
                        .ToList();
                });

                System.Diagnostics.Debug.WriteLine($"Загружено поставщиков: {supplierMaterials.Count}");

                SuppliersList.ItemsSource = supplierMaterials;
                SuppliersList.InvalidateVisual();
                SuppliersList.UpdateLayout();
            }
            catch (System.Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Ошибка загрузки поставщиков:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private async void CalcBtn_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductTypeCombo.SelectedItem is not ProductType selectedProduct)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Не выбран тип продукции.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                int productTypeId = selectedProduct.Id;
                int materialTypeId = material?.MaterialTypeId ?? -1;

                if (materialTypeId <= 0)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Не удалось определить тип материала.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                if (!int.TryParse(UsedAmountText.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var usedAmount))
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Неверное количество сырья. Введите целое число.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                if (!double.TryParse(Param1Text.Text, NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var p1))
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Неверный параметр 1. Введите положительное вещественное число.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                if (!double.TryParse(Param2Text.Text, NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var p2))
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Неверный параметр 2. Введите положительное вещественное число.", ButtonEnum.Ok).ShowAsync();
                    return;
                }

                var result = ProductionCalculator.CalculateProducedQuantity(productTypeId, materialTypeId, usedAmount, p1, p2);

                if (result < 0)
                {
                    CalcResultText.Text = "Ошибка расчёта (проверьте входные данные).";
                }
                else
                {
                    CalcResultText.Text = $"Ожидаемый выпуск: {result} шт. (при использовании {usedAmount} ед. сырья)";
                }
            }
            catch (System.Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Ошибка при расчёте:\n{ex.Message}", ButtonEnum.Ok).ShowAsync();
            }
        }

        private void BtnBack_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

