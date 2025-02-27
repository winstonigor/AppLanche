using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

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

    }

    private void BtnAdicionar_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncluirNoCarrinho_Clicked(object sender, EventArgs e)
    {

    }
}