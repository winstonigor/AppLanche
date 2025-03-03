namespace AppLanches.Pages;

public partial class PedidoConfirmadoPage : ContentPage
{
	public PedidoConfirmadoPage()
	{
		InitializeComponent();
	}

    private async void BtnRetornar_Clicked(object sender, EventArgs e)
    {
		//Remove a pagina atual da pilja de navegação e navega para a pagina anterior;
		await this.Navigation.PopAsync();
    }
}