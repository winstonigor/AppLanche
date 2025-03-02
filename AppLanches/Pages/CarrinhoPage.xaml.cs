using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;
using System.Collections.ObjectModel;

namespace AppLanches.Pages;

public partial class CarrinhoPage : ContentPage
{
    private ObservableCollection<CarrinhoCompraItem>? _ListCarrinhoCompra;
    public ObservableCollection<CarrinhoCompraItem>? ListCarrinhoCompra
    {
        get { return _ListCarrinhoCompra; }
        set { _ListCarrinhoCompra = value; }
    }

    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public CarrinhoPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        this.ListCarrinhoCompra = new ObservableCollection<CarrinhoCompraItem>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await this.GetItensCarrinhoCompra();
    }

    private async Task<IEnumerable<CarrinhoCompraItem>> GetItensCarrinhoCompra()
    {
        try
        {
            var usuarioId = Preferences.Get("usuarioid", 0);
            var (itensCarrinhoCompra, errorMessage) = await _apiService.GetItensCarrinhoCompra(usuarioId);

            if(errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await this.DisplayLoginPage();
                return Enumerable.Empty<CarrinhoCompraItem>();
            }

            if(itensCarrinhoCompra is null)
            {
                await this.DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os itens do carrinho", "OK");
                return Enumerable.Empty<CarrinhoCompraItem>();
            }

            this.ListCarrinhoCompra?.Clear();
            foreach (var item in itensCarrinhoCompra)
            {
                this.ListCarrinhoCompra?.Add(item);
            }

            this.CvCarrinho.ItemsSource = this.ListCarrinhoCompra;
            this.AtualizaPrecoTotal();

            return itensCarrinhoCompra;
        }
        catch (Exception ex)
        {
            await this.DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<CarrinhoCompraItem>();            
        }
    }

    private void AtualizaPrecoTotal()
    {
        try
        {
            var precoTotal = this.ListCarrinhoCompra?.Sum(s => s.Preco * s.Quantidade);
            this.LblPrecoTotal.Text = precoTotal.ToString();
        }
        catch (Exception ex)
        {
            this.DisplayAlert("Erro", $"Ocorreu um erro ao atualizar preco total: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await this.Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}