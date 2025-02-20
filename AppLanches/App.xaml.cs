﻿using AppLanches.Pages;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        public App(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;

            this.Inicializa();
        }

        private void Inicializa()
        {
            //Captura o token amarzenado
            var accessToken = Preferences.Get("accesstoken", string.Empty);
            if (string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new InscricaoPage(_apiService, _validator));
                return;
            }

            MainPage = new AppShell();
        }
    }
}
