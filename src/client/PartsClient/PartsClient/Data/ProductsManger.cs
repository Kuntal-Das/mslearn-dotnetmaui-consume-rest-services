using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PartsClient.Data
{
    internal class ProductsManger
    {
        private const string BaseAddress = "https://dummyjson.com/";
        private static string _accessToken = "";
        private static string _refreshToken = "";
        private static HttpClient _client;

        private static async Task<HttpClient> GetClient()
        {
            if (_client != null)
            {
                return _client;
            }

            _client = new HttpClient(new SocketsHttpHandler()
            {
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5)
            })
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (string.IsNullOrEmpty(_accessToken))
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "auth/login");
                request.Content = JsonContent.Create(new User
                {
                    Username = "emilys",
                    Password = "emilyspass"
                });
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var authenticatedUser = await response.Content.ReadFromJsonAsync<User>();
                _accessToken = authenticatedUser.AccessToken;
                _refreshToken = authenticatedUser.RefreshToken;
            }

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            return _client;
        }

        public static async Task RefreshToken()
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                refreshToken = _refreshToken
            }), Encoding.UTF8, "application/json");
            var response = await _client.PostAsJsonAsync("https://dummyjson.com/auth/refresh", jsonContent);
            response.EnsureSuccessStatusCode();
            var responseJsonObj = JsonNode.Parse(await response.Content.ReadAsStringAsync());
            if (responseJsonObj != null)
            {
                _accessToken = responseJsonObj["accessToken"]?.ToString();
                _refreshToken = responseJsonObj["refreshToken"]?.ToString();
            }
        }

        public static async Task<IEnumerable<Product>> GetAll()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return new List<Product>();

            var client = await GetClient();
            string result = await client.GetStringAsync("auth/products");
            var root = JsonNode.Parse(result);

            return root["products"]?.Deserialize<List<Product>>();
        }

        public static async Task<Product> Add(string productTitle, string productTags, string brand)
        {
            var product = new Product()
            {
                Title = productTitle,
                Tags = productTags.Split(',').ToList(),
                Brand = brand
            };
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return new Product();

            var client = await GetClient();
            var msg = new HttpRequestMessage(HttpMethod.Post, "auth/products/add");
            msg.Content = JsonContent.Create<Product>(product);
            var response = await client.SendAsync(msg);
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var insertedProduct = JsonSerializer.Deserialize<Product>(returnedJson);

            return insertedProduct;
        }

        public static async Task Update(Product product)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            var msg = new HttpRequestMessage(HttpMethod.Put, $"auth/products/{product.Id}");
            msg.Content = JsonContent.Create(product);
            var client = await GetClient();
            var response = await client.SendAsync(msg);

            // var jsonContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            // var response = await client.PostAsJsonAsync($"auth/products/{product.Id}", jsonContent);
            response.EnsureSuccessStatusCode();
        }

        public static async Task Delete(string partID)
        {
            throw new NotImplementedException();
        }
    }
}