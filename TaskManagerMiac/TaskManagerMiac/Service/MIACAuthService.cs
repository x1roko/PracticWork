using System.Text.Json;
using TaskManagerMiac.Dto.AuthDto;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace TaskManagerMiac.Service
{
    public class MIACAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private const string _testUrl = "http://localhost:5031/GetAdminUser"; // Тестовое АПИ
        private string _url; // АПИ с возвратом ссылки на госуслуги
        private string _userUrl; // АПИ с возвратом пользователя
        private string _appName;
        public MIACAuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _userUrl = Environment.GetEnvironmentVariable("GET_USER_API") ?? _configuration["GetUserAPI"] ?? "";
            _url = Environment.GetEnvironmentVariable("ESIA_LOGIN_API") ?? _configuration["EsiaLoginAPI"] ?? "";
            _appName = Environment.GetEnvironmentVariable("APP_NAME") ?? _configuration["AppName"] ?? "";
        }

        /// <summary>
        /// Метод для получения ссылки на госуслуги
        /// </summary>
        public async Task<AuthEsiaUrl> GetURL()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _url);
            httpRequestMessage.Headers.Add("app_name", _appName);
            AuthEsiaUrl? authUrl = null;
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    authUrl = await JsonSerializer.DeserializeAsync<AuthEsiaUrl>(contentStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MIACAuthService: " + ex.Message);
            }
            return authUrl;
        }

        /// <summary>
        /// Метод для получения пользователя через госуслуги
        /// </summary>
        /// <param name="code">Возвращают госуслуги</param>
        /// <param name="state">Возвращают госуслуги</param>
        /// <returns></returns>
        public async Task<AuthUserDto> AuthorizeUserFromEsiaApi(string code, string state)
        {
            AuthUserDto? authUser = null;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _userUrl);
            httpRequestMessage.Headers.Add("app_name", _appName);
            httpRequestMessage.Headers.Add("code", code);
            httpRequestMessage.Headers.Add("state", state);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    authUser = (await JsonSerializer.DeserializeAsync<AuthUser>(contentStream)).user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MIACAuthService: " + ex.Message);
            }
            return authUser;
        }

        /// <summary>
        /// Старый метод для авторизации через тестовое АПИ
        /// </summary>
        /// <param name="url">Ссылка на АПИ с нужным методом</param>
        /// <returns></returns>
        public async Task<AuthUserDto> AuthorizeUserFromTestApi(string url = _testUrl)
        {
            AuthUserDto? authUser = null;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    authUser = await JsonSerializer.DeserializeAsync<AuthUserDto>(contentStream);
                }              
            }
            catch (Exception ex)
            {
                Console.WriteLine("MIACAuthService: "+ex.Message);
            }
            return authUser;
        }
    }
}

