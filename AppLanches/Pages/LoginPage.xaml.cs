using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public LoginPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (await this.Valida())
        {
            var responde = await _apiService.Login(this.EntEmail.Text, this.EntPassword.Text);
            if (!responde.HasError)
            {
                Application.Current!.MainPage = new AppShell(_apiService, _validator);
            }
            else
                await DisplayAlert("Atenção", "Algo deu errado", "Ok");
        }
    }

    private async Task<bool> Valida()
    {
        if (!await this.ValidaEmail()) return false;
        if (!await this.ValidaSenha()) return false;
        return true;
    }

    private async Task<bool> ValidaSenha()
    {
        if (string.IsNullOrEmpty(this.EntPassword.Text))
        {
            await this.DisplayAlert("Atenção", "Informe a senha", "Ok");
            this.EntPassword.Focus();
            return false;
        }
        return true;
    }
    private async Task<bool> ValidaEmail()
    {
        if (string.IsNullOrEmpty(this.EntEmail.Text))
        {
            await DisplayAlert("Atenção", "Informe o email", "OK");
            this.EntEmail.Focus();
            return false;
        }
        return true;
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InscricaoPage(_apiService, _validator));
    }
}