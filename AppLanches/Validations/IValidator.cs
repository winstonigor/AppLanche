namespace AppLanches.Validations
{
    public interface IValidator
    {
        public string NomeErro { get; set; }
        public string EmailErro { get; set; }
        public string TelefoneErro { get; set; }
        public string SenhaErro { get; set; }
        Task<bool> Validar(string nome, string email, string telefone, string senha);
    }
}