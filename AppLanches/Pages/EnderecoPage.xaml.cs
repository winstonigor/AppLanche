namespace AppLanches.Pages;

public partial class EnderecoPage : ContentPage
{
	public EnderecoPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.CarregaDadosSalvoSalvo();
    }

    private void CarregaDadosSalvoSalvo()
    {
        if (Preferences.ContainsKey("nome"))
            this.EntNome.Text = Preferences.Get("nome", string.Empty);

        if (Preferences.ContainsKey("endereco"))
            this.EntEndereco.Text = Preferences.Get("endereco", string.Empty);

        if (Preferences.ContainsKey("telefone"))
            this.EntNome.Text = Preferences.Get("telefone", string.Empty);
    }

    private void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("nome", this.EntNome.Text);
        Preferences.Set("endereco", this.EntEndereco.Text);
        Preferences.Set("telefone", this.EntTelefone.Text);
        this.Navigation.PopAsync();
    }
}