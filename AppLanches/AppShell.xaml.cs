using AppLanches.Pages;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            this.ManipulaInterface();
        }

        //Cria as navegações via C#
        private void ManipulaInterface()
        {
            var homePage = new HomePage(_apiService, _validator);
            var carrinhoPage = new CarrinhoPage();
            var favoritoPage = new FavoritosPage();
            var perfilPage = new PerfilPage();

            Items.Add(new TabBar
            {
                Items = 
                {
                    new ShellContent { Title = "Home", Icon = "home", Content = homePage },
                    new ShellContent { Title = "Carrinho", Icon = "cart", Content = carrinhoPage },
                    new ShellContent { Title = "Favorito", Icon = "heart", Content = favoritoPage },
                    new ShellContent { Title = "Perfil", Icon = "profile", Content = perfilPage },
                }
            });
        }
    }
}
