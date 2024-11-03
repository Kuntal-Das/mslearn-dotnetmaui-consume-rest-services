using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PartsClient.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PartsClient.ViewModels;

public partial class PartsViewModel : ObservableObject
{
    [ObservableProperty]
    //ObservableCollection<Part> _parts;
    ObservableCollection<Product> _parts;


    [ObservableProperty]
    bool _isRefreshing = false;


    [ObservableProperty]
    bool _isBusy = false;


    [ObservableProperty]
    //Part _selectedPart;
    Product _selectedProduct;

    public PartsViewModel()
    {
        //_parts = new ObservableCollection<Part>();
        _parts = new ObservableCollection<Product>();

        WeakReferenceMessenger.Default.Register<RefreshMessage>(this, async (r, m) =>
        {
            await LoadData();
        });

        Task.Run(LoadData);
    }

    [RelayCommand]
    async Task PartSelected()
    {
        if (SelectedProduct == null)
            return;

        var navigationParameter = new Dictionary<string, object>()
        {
            { "part", SelectedProduct }
        };

        await Shell.Current.GoToAsync("addpart", navigationParameter);

        MainThread.BeginInvokeOnMainThread(() => SelectedProduct = null);
    }

    [RelayCommand]
    async Task LoadData()
    {
        if (IsBusy)
            return;

        try
        {
            IsRefreshing = true;
            IsBusy = true;

            //var partsCollection = await PartsManager.GetAll();
            var productsCollecton = await ProductsManger.GetAll();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Parts.Clear();

                //foreach (Part part in partsCollection)
                foreach (var part in productsCollecton)
                {
                    Parts.Add(part);
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsRefreshing = false;
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddNewPart()
    {
        await Shell.Current.GoToAsync("addpart");
    }

}
