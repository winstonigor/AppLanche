using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppLanches.Validations
{
    public class Validator : IValidator
    {
        public string NomeErro { get; set; } = "";
        public string EmailErro { get; set; } = "";
        public string TelefoneErro { get; set; } = "";
        public string SenhaErro { get; set; } = "";

        private const string NomeVazioErroMsg = "Por favor, informe o seu nome.";
        private const string NomeInvalidoErroMsg = "Por favor, informe um nome válido.";
        private const string EmailVazioErroMsg = "Por favor, informe um email.";
        private const string EmailInvalidoErroMsg = "Por favor, informe um email válido.";
        private const string TelefoneVazioErroMsg = "Por favor, informe um telefone.";
        private const string TelefoneInvalidoErroMsg = "Por favor, informe um telefone válido.";
        private const string SenhaVazioErroMsg = "Por favor, informe a senha.";
        private const string SenhaInvalida = "A senha deve conter pelo menos 8 caracteres, incluindo letras e números.";

        public Task<bool> Validar(string nome, string email, string telefone, string senha)
        {
            var isNomeValido = ValidarNome(nome);
            var isEmailValido = Validaremail(email);
            var isTelefoneValido = ValidarTelefone(telefone);
            var isSenhaValido = ValidarSenha(senha);


            return Task.FromResult(isNomeValido && isEmailValido && isTelefoneValido && isSenhaValido);
        }

        private bool ValidarSenha(string senha)
        {
            if (string.IsNullOrEmpty(senha))
            {
                this.SenhaErro = SenhaVazioErroMsg;
                return false;
            }
            if(senha.Length < 8 || !Regex.IsMatch(senha, @"[a-zA-Z]") || !Regex.IsMatch(senha, @"\d"))
            {
                this.SenhaErro = SenhaInvalida;
                return false;
            }

            SenhaErro = string.Empty;
            return true;
        }

        private bool ValidarTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
            {
                this.TelefoneErro = TelefoneVazioErroMsg;
                return false;
            }
            if(telefone.Length < 12)
            {
                this.TelefoneErro = TelefoneInvalidoErroMsg;
                return false;
            }

            this.TelefoneErro = string.Empty;
            return true;
        }

        private bool Validaremail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailErro = EmailVazioErroMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailErro = EmailInvalidoErroMsg;
                return false;   
            }

            this.EmailErro = string.Empty;
            return true;
        }

        private bool ValidarNome(string nome)
        {
            if (string.IsNullOrEmpty(nome))
            {
                NomeErro = NomeVazioErroMsg;
                return false;   
            }

            if(nome.Length < 3)
            {
                NomeErro = NomeInvalidoErroMsg;
                return false;
            }

            this.NomeErro = string.Empty;
            return true;
        }
    }
}
