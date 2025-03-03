using AppLanches.Models;
using AppLanches.Pages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppLanches.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://pht5ls1r-7066.brs.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;
        JsonSerializerOptions _serializerOptions;
        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        }
        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            try
            {
                var enderecoUrl = _baseUrl + uri;
                return await _httpClient.PostAsync(enderecoUrl, content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar requisição POST para {uri}:{ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endPoint)
        {
            try
            {
                this.AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endPoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Erro de desserialização JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }
        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            try
            {
                var endrecoUrl = AppConfig.BaseUrl + uri;
                this.AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(endrecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar requisição PUT para {uri}:{ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("Accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse<bool>> RegisterUser(string nome, string email, string telefone, string password)
        {
            try
            {
                var register = new Register()
                {
                    Nome = nome,
                    Email = email,
                    Telefone = telefone,
                    Senha = password
                };
                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await PostRequest("api/Usuarios/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>()
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao registrar o usuario: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }
        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Usuario/Login", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("usuarioid", (int)result.UsuarioId!);
                Preferences.Set("usuarionome", result.UsuarioNome);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to login {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }
        public async Task<(List<Categoria>? Categorias, string? ErrorMessage)> GetCategorias()
        {
            return await GetAsync<List<Categoria>>("api/categoria");
        }
        public async Task<(List<Produto>? Produtos, string? ErrorMessage)> GetProdutos(string tipoProduto, string categoriaId)
        {
            var endpoint = $"api/Produtos?tipoProduto={tipoProduto}&categoriaId={categoriaId}";
            return await this.GetAsync<List<Produto>>(endpoint);
        }
        public async Task<(Produto? ProdutoDetalhe, string? ErrorMessage)> GetProdutoDetalhe(int produtoId)
        {
            string endpoint = $"api/produtos/{produtoId}";
            return await this.GetAsync<Produto>(endpoint);
        }
        public async Task<ApiResponse<bool>> AdicionaItemNoCarrinho(CarrinhoCompra carrinhoCompra)
        {
            try
            {
                var json = JsonSerializer.Serialize(carrinhoCompra, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await this.PostRequest("api/ItensCarrinhoCompra", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}" };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicoinar item no carrinho: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }
        public async Task<(List<CarrinhoCompraItem>? carrinhoCompraItems, string? ErrorMessage)> GetItensCarrinhoCompra(int usuarioId)
        {
            var endpoint = $"api/ItensCarrinhoCompra/{usuarioId}";
            return await this.GetAsync<List<CarrinhoCompraItem>>(endpoint);
        }
        public async Task<(bool Data, string? ErrorMessage)> AtualizaQuantidadeItemCarrinho(int produtoId, string acao)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await this.PutRequest($"api/ItensCarrinhoCompra?produtoId={produtoId}&acao={acao}", content);
                if (response.IsSuccessStatusCode)
                    return (true, null);
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (false, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (false, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"Erro de requisição HTTP: {ex.Message}";
                _logger.LogError(errorMessage);
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
        }

        public async Task<ApiResponse<bool>> ConfirmarPedido(Pedido pedido)
        {
            try
            {
                var json = JsonSerializer.Serialize(pedido, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await this.PostRequest("api/Pedidos", content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthrized"
                        : $"Erro ao enviar requisição HTTP: {response.StatusCode}";

                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao confirmar pedido: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

    }
}
