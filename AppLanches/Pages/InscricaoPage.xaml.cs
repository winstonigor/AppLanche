using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class InscricaoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public InscricaoPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
    private async void btnSignup_Clicked(object sender, EventArgs e)
    {
        if (await _validator.Validar(EntNome.Text, EntEmail.Text, EntTelefone.Text, this.EntPassword.Text))
        {
            var response = await _apiService.RegisterUser(EntNome.Text, EntEmail.Text, EntTelefone.Text, this.EntPassword.Text);
            if (!response.HasError)
            {
                await DisplayAlert("Atenção", "Sua conta foi criada com sucesso!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Atenção", "Algo deu errado!", "OK");
            }
        }
        else
        {
            string mensagemErro = "";
            mensagemErro += _validator.NomeErro != null ? $"\n- {_validator.NomeErro}" :string.Empty;
            mensagemErro += _validator.EmailErro != null ? $"\n- {_validator.EmailErro}" :string.Empty;
            mensagemErro += _validator.TelefoneErro != null ? $"\n- {_validator.TelefoneErro}" :string.Empty;
            mensagemErro += _validator.SenhaErro != null ? $"\n- {_validator.SenhaErro}" :string.Empty;

            await DisplayAlert("Atenão", mensagemErro, "Ok");
        }
        
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InscricaoPage(_apiService, _validator));
    }
}