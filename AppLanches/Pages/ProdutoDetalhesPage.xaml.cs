using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;
using System.Runtime.CompilerServices;

namespace AppLanches.Pages;

public partial class ProdutoDetalhesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private readonly int _produtoId;
    private int _categoriaId;
    public ProdutoDetalhesPage(int produtoId, string produtoNome, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _produtoId = produtoId;
        this.Title = produtoNome ?? "Detalhe do Produto";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await this.GetProdutoDetalhes(_produtoId);
    }

    private async Task<Produto?> GetProdutoDetalhes(int produtoId)
    {
        var (produtoDetalhe, errorMessage) = await _apiService.GetProdutoDetalhe(produtoId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (produtoDetalhe is null)
        {
            await this.DisplayAlert("Erro", errorMessage ?? "Não foi possível obter o produto.", "OK");
            return null;
        }

        if (produtoDetalhe != null)
        {
            this.ImagemProduto.Source = produtoDetalhe.CaminhoImagem;
            this.LblProdutoNome.Text = produtoDetalhe.Nome;
            this.LblProdutoPreco.Text = produtoDetalhe.Preco.ToString();
            this.LblProdutoDescricao.Text = produtoDetalhe.Detalhe;
            this.LblPrecoTotal.Text = produtoDetalhe.Preco.ToString();
        }
        else
        {
            await this.DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os detalhes do produto.", "OK");
            return null;
        }

        return produtoDetalhe;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await this.Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void ImagemBtnFavorido_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnRemover_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(this.LblQuantidade.Text, out int quantidade) &&
            decimal.TryParse(this.LblProdutoPreco.Text, out decimal precoUnitario))
        {
            //decrementa a quantidade, e não premite que seja menor que 1
            quantidade = Math.Max(1, quantidade - 1);
            this.LblQuantidade.Text = quantidade.ToString();

            //calcula o preço total
            var precoTotal = quantidade * precoUnitario;
            this.LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            DisplayAlert("Erro", "Valores Inválidos", "OK");
        }
    }

    private void BtnAdicionar_Clicked(object sender, EventArgs e)
    {
        if(int.TryParse(this.LblQuantidade.Text, out int quantidade) &&
            decimal.TryParse(this.LblProdutoPreco.Text, out decimal precoUnitario))
        {
            //incrementa quantidade
            quantidade++;
            this.LblQuantidade.Text = quantidade.ToString();

            //calcula o preco total
            var precoTotal = quantidade * precoUnitario;
            this.LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            //error
            this.DisplayAlert("Erro", "Valores Inválidos", "OK");
        }
    }

    private async void BtnIncluirNoCarrinho_Clicked(object sender, EventArgs e)
    {
        try
        {
            var carrinhoCompra = new CarrinhoCompra()
            {
                Quantidade = Convert.ToInt32(this.LblQuantidade.Text),
                PrecoUnitario = Convert.ToDecimal(this.LblProdutoPreco.Text),
                ValorTotal = Convert.ToDecimal(this.LblPrecoTotal.Text),
                ProdutoId = _produtoId,
                ClienteId = Preferences.Get("usuarioid", 0)
            };

            var response = await _apiService.AdicionaItemNoCarrinho(carrinhoCompra);
            if (response.Data)
            {
                await this.DisplayAlert("Sucesso", "Item adicionado ao carrinho!", "OK");
                
                //volta uma pagina (pagina anterior)
                await this.Navigation.PopAsync();
            }
            else
            {
                await this.DisplayAlert("Erro", $"Falha ao adicoinar item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await this.DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }
}