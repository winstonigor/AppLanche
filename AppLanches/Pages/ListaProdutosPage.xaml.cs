using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ListaProdutosPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator _validator;
	private bool _loginPageDisplayed = false;
	private int _categoriaId;

	public ListaProdutosPage(int categoriaId, string categoriaNome, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
		_apiService = apiService;
		_validator = validator;
		_categoriaId = categoriaId;
		this.Title = categoriaNome ?? "Produtos";
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await this.GetListaProdutos(_categoriaId);
	}

	private async Task DisplayLoginPage()
	{
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
	}

	private async Task<IEnumerable<Produto>> GetListaProdutos(int categoriaId)
	{
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("categoria", categoriaId.ToString());
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                //Chama a pagina de login
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter as categorias.", "Ok");
                return Enumerable.Empty<Produto>();
            }

            //popula o colection View
            this.CvProdutos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }
}