using AppLanches.Services;

namespace AppLanches.Pages;

public partial class InscricaoPage : ContentPage
{
    private readonly ApiService _apiService;
    public InscricaoPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }
    private async void btnSignup_Clicked(object sender, EventArgs e)
    {
        var response = await _apiService.RegisterUser(EntNome.Text, EntEmail.Text, EntTelefone.Text, this.EntPassword.Text);
        if (!response.HasError)
        {
            await DisplayAlert("Atenção", "Sua conta foi criada com sucesso!", "OK");
            await Navigation.PushAsync(new LoginPage(_apiService));
        }
        else
        {
            await DisplayAlert("Atenção", "Algo deu errado!", "OK");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService));
    }
}