using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Models
{
    public class CarrinhoCompraItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id { get; set; }
        public decimal Preco { get; set; }
        public decimal ValorTotal { get; set; }

        private int _Quantidade;
        public int Quantidade
        {
            get { return _Quantidade; }
            set
            {
                if (_Quantidade != value)
                {
                    _Quantidade = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int ProdutoId { get; set; }
        public string? ProdutoNome { get; set; }
        public string? UrlImagem { get; set; }
        public string? CaminhoImagem => AppConfig.BaseUrl + UrlImagem;

    }
}
