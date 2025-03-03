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
    private bool _isNavigatingToEmptyCartPage = false;

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
        if (this.IsNavigatingToEmptyCartPage()) return;

        bool hasItems = await this.GetItensCarrinhoCompra();

        if (hasItems)
            this.ExibirEndereco();
        else
            await this.NavegarParaCarrinhoVazio();
    }

    private async Task NavegarParaCarrinhoVazio()
    {
        this.LblEndereco.Text = string.Empty;
        this._isNavigatingToEmptyCartPage = true;
        await this.Navigation.PushAsync(new CarrinhoVazioPage(_apiService, _validator));
    }

    private bool IsNavigatingToEmptyCartPage()
    {
        if (_loginPageDisplayed)
        {
            _isNavigatingToEmptyCartPage = false;
            return true;
        }
        
        return false;
    }

    private void ExibirEndereco()
    {
        var enderecoSalvo = Preferences.ContainsKey("endereco");
        if (enderecoSalvo)
        {
            var nome = Preferences.Get("nome", string.Empty);
            var endereco = Preferences.Get("endereco", string.Empty);
            var telefone = Preferences.Get("telefone", string.Empty);

            this.LblEndereco.Text = $"{nome}\n{endereco} \n{telefone}";
        }
        else
        {
            this.LblEndereco.Text = "Informe o seu endereço";
        }
    }

    private async Task<bool> GetItensCarrinhoCompra()
    {
        try
        {
            var usuarioId = Preferences.Get("usuarioid", 0);
            var (itensCarrinhoCompra, errorMessage) = await _apiService.GetItensCarrinhoCompra(usuarioId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await this.DisplayLoginPage();
                return false;
            }

            if (itensCarrinhoCompra is null)
            {
                await this.DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os itens do carrinho", "OK");
                return false;
            }

            this.ListCarrinhoCompra?.Clear();
            foreach (var item in itensCarrinhoCompra)
            {
                this.ListCarrinhoCompra?.Add(item);
            }

            this.CvCarrinho.ItemsSource = this.ListCarrinhoCompra;
            this.AtualizaPrecoTotal();

            if (!this.ListCarrinhoCompra.Any())
                return false;

            return true;
        }
        catch (Exception ex)
        {
            await this.DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return false;
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

    private async void BtnIncrementar_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            itemCarrinho.Quantidade++;
            this.AtualizaPrecoTotal();
            await _apiService.AtualizaQuantidadeItemCarrinho(itemCarrinho.ProdutoId, "aumentar");
        }
    }

    private async void BtnDecrementar_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            if (itemCarrinho.Quantidade == 1)
                return;
            else
            {
                itemCarrinho.Quantidade--;
                this.AtualizaPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(itemCarrinho.ProdutoId, "diminuir");
            }
        }
    }

    private async void BtnDeletar_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            var resposta = await this.DisplayAlert("Confirmação", "Tem certeza que deseja excluir o item do carrinho?", "Sim", "Não");
            if (resposta)
            {
                this.ListCarrinhoCompra?.Remove(itemCarrinho);
                this.AtualizaPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(itemCarrinho.ProdutoId, "deletar");
            }
        }
    }

    private void BtnEditaEndereco_Clicked(object sender, EventArgs e)
    {
        this.Navigation.PushAsync(new EnderecoPage());
    }

    private async void TapConfirmarPedido_Tapped(object sender, TappedEventArgs e)
    {
        if (this.ListCarrinhoCompra == null || !this.ListCarrinhoCompra.Any())
        {
            await this.DisplayAlert("Informação", "Seu carrinho está vazio ou já foi confirmado", "OK");
            return;
        }

        this.ConfirmaPedido();
    }

    private async void ConfirmaPedido()
    {
        var pedido = new Pedido()
        {
            Endereco = this.LblEndereco.Text,
            UsuarioId = Preferences.Get("usuarioid", 0),
            ValorTotal = Convert.ToDecimal(this.LblPrecoTotal.Text)
        };

        var response = await _apiService.ConfirmarPedido(pedido);
        if (response.HasError)
        {
            if (response.ErrorMessage == "Unauthorized")
            {
                await this.DisplayLoginPage();
                return;
            }
            await this.DisplayAlert("Atenção", $"Algo deu errado: {response.ErrorMessage}", "Cancelar");
            return;
        }

        this.ListCarrinhoCompra?.Clear();
        this.LblEndereco.Text = "Informe o seu endereço";
        this.LblPrecoTotal.Text = "0.00";

        await this.Navigation.PushAsync(new PedidoConfirmadoPage());
    }
}