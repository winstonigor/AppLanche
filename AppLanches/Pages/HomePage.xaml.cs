using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _dadosCarregado = false;

    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _validator = validator;
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        this.lblNomeUsuario.Text = "Ol�," + Preferences.Get("usuarionome", string.Empty);
        this.Title = AppConfig.tituloHomePage;
    }

    //Utilizado quando a pagina est� preste a ser exibida
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_dadosCarregado)
        {
            await this.CarregaDadados();
            this._dadosCarregado = true;
        }
    }

    private async Task CarregaDadados()
    {
        var categoriasTask = this.GetListaCategorias();
        var maisVendicosTask = this.GetMaisVendidos();
        var popularesTask = this.GetPopulares();

        await Task.WhenAll(categoriasTask, maisVendicosTask, popularesTask);
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
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter os produtos populares.", "Ok");
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
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter os produtos mais vendidos.", "Ok");
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
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter as categorias.", "Ok");
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

    private void CvCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var categoriaSelecionada = e.CurrentSelection.FirstOrDefault() as Categoria;
        if (categoriaSelecionada == null) return;

        Navigation.PushAsync(new ListaProdutosPage(categoriaSelecionada.Id, categoriaSelecionada.Nome!, _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;

    }

    private void CvPopulares_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
            this.NavigateToProdutoDetalhesPage(collectionView, e);
    }

    private void CvMaisVendidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
            this.NavigateToProdutoDetalhesPage(collectionView, e);
    }

    private void NavigateToProdutoDetalhesPage(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var itemSelecionado = e.CurrentSelection.FirstOrDefault() as Produto;
        if (itemSelecionado is null)
            return;

        this.Navigation.PushAsync(new ProdutoDetalhesPage(itemSelecionado.Id, itemSelecionado.Nome!, _apiService, _validator));

        collectionView.SelectedItem = null;
    }
}