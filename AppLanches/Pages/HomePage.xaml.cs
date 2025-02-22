using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _validator = validator;
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        this.lblNomeUsuario.Text = "Olá," + Preferences.Get("usuarionome", string.Empty);
    }

    //Utilizado qiando a pagina está preste a ser exibida
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await this.GetListaCategorias();
        await this.GetMaisVendidos();
        await this.GetPopulares();
    }

    private async Task<IEnumerable<Produto>> GetPopulares()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("popular", string.Empty);
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                //Chama a pagina de login
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os produtos populares.", "Ok");
                return Enumerable.Empty<Produto>();
            }

            //popula o colection View
            this.CvPopulares.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }
    private async Task<IEnumerable<Produto>> GetMaisVendidos()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("maisvendidos", string.Empty);
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                //Chama a pagina de login
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os produtos mais vendidos.", "Ok");
                return Enumerable.Empty<Produto>();
            }

            //popula o colection View
            this.CvMaisVendidos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }
    private async Task<IEnumerable<Categoria>> GetListaCategorias()
    {
        try
        {
            var (categorias, errorMessage) = await _apiService.GetCategorias();
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                //Chama a pagina de login
                await DisplayLoginPage();
                return Enumerable.Empty<Categoria>();
            }

            if (categorias == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter as categorias.", "Ok");
                return Enumerable.Empty<Categoria>();
            }

            //popula o colection View
            this.CvCategorias.ItemsSource = categorias;
            return categorias;

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Categoria>();
        }
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}