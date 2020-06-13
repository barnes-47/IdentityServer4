using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleClient
{
    internal delegate Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage message);
    internal class Program
    {
        #region Internal Class
        internal static class Scope
        {
            internal const string All = "";
            internal const string Api1 = "api1";
            internal const string ApiHost_DeleteAccess = "api_host.delete_access";
            internal const string ApiHost_FullAccess = "api_host.full_access";
            internal const string ApiHost_ReadAccess = "api_host.read_access";
            internal const string ApiHost_WriteAccess = "api_host.write_access";
        }
        #endregion

        #region Private Constants
        private const string _ApiBaseAddress = "https://localhost:6001"; //ApiHost's hostname
        private const string _Authority = "https://localhost:5001";       //IdentityServer's hostname
        private const string _ClientId = "console";
        private const string _ClientSecret = "secret";

        private const ConsoleColor Green = ConsoleColor.Green;
        private const ConsoleColor Red = ConsoleColor.Red;
        private const ConsoleColor DarkGreen = ConsoleColor.DarkGreen;
        private const ConsoleColor DarkYellow = ConsoleColor.DarkYellow;
        private const ConsoleColor Default = ConsoleColor.Gray;
        private const ConsoleColor White = ConsoleColor.White;
        private const ConsoleColor Yellow = ConsoleColor.Yellow;
        
        #endregion

        static void Main(string[] args)
            => MainAsync(args).GetAwaiter().GetResult();

        public static async Task MainAsync(string[] args)
        {
            var scopes = new[]
            {
                Scope.ApiHost_DeleteAccess,
                Scope.ApiHost_FullAccess,
                Scope.ApiHost_ReadAccess,
                Scope.ApiHost_WriteAccess
            };

            foreach (var scope in scopes)
            {
                Write80WhiteAsteriskLine();
                //Get access token.
                var accessToken = await GetAccessTokenAsync(scope);
                if (string.IsNullOrEmpty(accessToken))
                    continue;

                //Every token call all the APIs whether they are (in)accessible by it.
                await CallAllTheApiAsync(accessToken);
                WriteLine();
                Write80WhiteAsteriskLine();
                WriteLine("\n\n");
            }

            ReadKey();
        }

        #region Private Methods        
        /// <summary>
        /// Creates a HttpClient instance and makes the actual call to the api(ApiHost in this case).
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="message">The message.</param>
        private static async Task CallApiAsync(string endpoint, string accessToken, HttpRequestMessage message)
        {
            WriteLine();
            SetTextColorTo(Yellow);
            WriteLine($"API HOST Response for {endpoint}:-");

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.SendAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                SetTextColorTo(Red);
                WriteLine($"StatusCode: {response.StatusCode}");
                WriteLine($"ReasonPhrase: {response.ReasonPhrase}");

                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            SetTextColorTo(DarkGreen);
            WriteLine($"StatusCode: {response.StatusCode}");
            WriteLine($"Content:    {JObject.Parse(content)}");
        }
        private static async Task CallCreateAsync(string accessToken)
        {
            var bodyObject = new { value = "value_created" };
            var json = JsonConvert.SerializeObject(bodyObject);

            //Create HttpRequestMessage instance.
            var requestMsg = new HttpRequestMessage
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                RequestUri = new Uri($"{_ApiBaseAddress}/api/values"),
                Method = HttpMethod.Post
            };

            //Call the api.
            await CallApiAsync("CREATE", accessToken, requestMsg);
        }
        private static async Task CallDeleteAsync(string accessToken)
        {
            //Create HttpRequestMessage instance.
            var requestMsg = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_ApiBaseAddress}/api/values/123"),
                Method = HttpMethod.Delete
            };

            //Call the api.
            await CallApiAsync("DELETE", accessToken, requestMsg);
        }
        private static async Task CallGetAsync(string accessToken)
        {
            //Create HttpRequestMessage instance.
            var requestMsg = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_ApiBaseAddress}/api/values/1123"),
                Method = HttpMethod.Get
            };

            //Call the api.
            await CallApiAsync("GET", accessToken, requestMsg);
        }
        private static async Task CallGetAllAsync(string accessToken)
        {
            //Create HttpRequestMessage instance.
            var requestMsg = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_ApiBaseAddress}/api/values"),
                Method = HttpMethod.Get
            };

            //Call the api.
            await CallApiAsync("GET ALL", accessToken, requestMsg);
        }
        private static async Task CallUpdateAsync(string accessToken)
        {
            var bodyObject = new { value = "value_updated" };
            var json = JsonConvert.SerializeObject(bodyObject);

            //Create HttpRequestMessage instance.
            var requestMsg = new HttpRequestMessage
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                RequestUri = new Uri($"{_ApiBaseAddress}/api/values"),
                Method = HttpMethod.Put
            };

            //Call the api.
            await CallApiAsync("UPDATE", accessToken, requestMsg);
        }
        private static async Task CallAllTheApiAsync(string accessToken)
        {
            await CallGetAllAsync(accessToken);
            await CallGetAsync(accessToken);
            await CallDeleteAsync(accessToken);
            await CallCreateAsync(accessToken);
            await CallUpdateAsync(accessToken);
        }
        private static async Task<string> GetAccessTokenAsync(string scope)
        {
            WriteLine();
            SetTextColorTo(Yellow);
            WriteLine($"IDENTITYSERVER4 HOST Response for '{scope}':-");

            //Create HTTP client instance.
            var httpClient = new HttpClient();

            //Get the discovery document by calling the extension method
            //present in HttpClientDiscoveryExtensions(namespace: IdentityModel.Client)
            var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync(_Authority);
            if (discoveryDoc.IsError)
            {
                SetTextColorTo(Red);
                WriteLine("Error occurred while fetching discovery document.");
                WriteLine();
                WriteLine($"Error: {discoveryDoc.Error}");

                return null;
            }

            //Create token request model
            var tokenRequestModel = new ClientCredentialsTokenRequest
            {
                Address = discoveryDoc.TokenEndpoint,
                ClientId = _ClientId,
                ClientSecret = _ClientSecret,

                //If Scope is not defined, all scopes are returned from the IdentityServer.
                Scope = scope
            };

            //Since we are using ClientCredential for machine-to-machine interaction.
            var response = await httpClient.RequestClientCredentialsTokenAsync(tokenRequestModel);
            if (response.IsError)
            {
                SetTextColorTo(Red);
                WriteLine("Error occurred while requesting client credential token.");
                WriteLine();
                WriteLine($"Error: {response.Error}");

                return null;
            }

            SetTextColorTo(DarkGreen);
            WriteLine(response.Json);

            return response.AccessToken;
        }
        private static void SetTextColorTo(ConsoleColor color)
            => ForegroundColor = color;
        private static void Write80WhiteAsteriskLine()
        {
            SetTextColorTo(White);
            WriteLine(new string('*', 80));
            SetTextColorTo(Default);
        }
        #endregion
    }
}
