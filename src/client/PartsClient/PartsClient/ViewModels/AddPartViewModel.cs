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
        Part partToSave = new()
        {
            PartID = ProductId,
            PartName = ProductTitle,
            PartType = ProductBrand,
            Suppliers = ProductTags.Split(",").ToList()
        };

        await PartsManager.Update(partToSave);

        WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task DeletePart()
    {
        if (string.IsNullOrWhiteSpace(ProductId))
            return;

        await PartsManager.Delete(ProductId);

        WeakReferenceMessenger.Default.Send(new RefreshMessage(true));

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task DoneEditing()
    {
        await Shell.Current.GoToAsync("..");
    }
}