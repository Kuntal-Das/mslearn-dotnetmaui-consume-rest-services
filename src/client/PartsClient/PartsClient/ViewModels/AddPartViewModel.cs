using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PartsClient.Data;

namespace PartsClient.ViewModels;

public partial class AddPartViewModel : ObservableObject
{
    [ObservableProperty] string _productId;

    [ObservableProperty] string _productTitle;

    [ObservableProperty] string _productTags;

    [ObservableProperty] string _productBrand;
    internal Product _productToDisplay;

    public AddPartViewModel()
    {
    }

    [RelayCommand]
    async Task SaveData()
    {
        if (string.IsNullOrWhiteSpace(ProductId))
            await InsertPart();
        else
            await UpdatePart();
    }


    [RelayCommand]
    async Task InsertPart()
    {
        // await PartsManager.Add(PartName, Suppliers, PartType);
        ProductsManger.Add(ProductTitle, ProductTags, ProductBrand);

        WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }


    [RelayCommand]
    async Task UpdatePart()
    {
        Product productToSave = _productToDisplay ?? new();

        productToSave.Id = int.Parse(ProductId);
        productToSave.Title = ProductTitle;
        productToSave.Brand = ProductBrand;
        productToSave.Tags = ProductTags.Split(",").ToList();


        await ProductsManger.Update(productToSave);

        WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task DeletePart()
    {
        if (string.IsNullOrWhiteSpace(ProductId))
            return;

        await ProductsManger.Delete(ProductId);

        WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task DoneEditing()
    {
        await Shell.Current.GoToAsync("..");
    }
}