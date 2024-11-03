using PartsClient.Data;
using PartsClient.ViewModels;

namespace PartsClient.Pages;

[QueryProperty(nameof(ProductToDisplay), "part")]
public partial class AddPartPage : ContentPage
{
    AddPartViewModel viewModel;

    public AddPartPage()
    {
        InitializeComponent();

        viewModel = new AddPartViewModel();
        BindingContext = viewModel;
    }

    // Part _partToDisplay;
    Product _productToDisplay;

    public Product ProductToDisplay
    {
        get => _productToDisplay;
        set
        {
            if (_productToDisplay == value)
                return;

            _productToDisplay = value;

            viewModel._productToDisplay = _productToDisplay;
            viewModel.ProductId = _productToDisplay.Id.ToString().PadLeft(8,'0');
            viewModel.ProductTitle = _productToDisplay.Title;
            viewModel.ProductTags = _productToDisplay.TagsAsString;
            viewModel.ProductBrand = _productToDisplay.Brand;
        }
    }
}