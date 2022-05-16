//@BaseCode
//MdStart
using QuickTemplate.Logic.Modules.Exceptions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace QuickTemplate.Logic.Modules.RestApi
{
    public partial class ClientAccess
    {
        protected static string MediaType => "application/json";
        protected static JsonSerializerOptions DeserializerOptions => new() { PropertyNameCaseInsensitive = true };
        protected static HttpClient CreateClient(string baseAddress)
        {
            HttpClient client = new();

            if (baseAddress.HasContent())
            {
                if (baseAddress.EndsWith(@"/") == false
                    && baseAddress.EndsWith(@"\") == false)
                {
                    baseAddress += "/";
                }

                client.BaseAddress = new Uri(baseAddress);
            }
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            return client;
        }
#if ACCOUNT_ON
        protected static HttpClient CreateClient(string baseAddress, string sessionToken)
        {
            HttpClient client =CreateClient(baseAddress);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sessionToken}")));

            return client;
        }
#endif
        public static async Task<T[]> GetAsync<T>(string baseUri, string extUri)
        {
            using var client = CreateClient(baseUri);
            var response = await client.GetAsync($"{extUri}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var contentData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<T[]>(contentData, DeserializerOptions).ConfigureAwait(false);

                return result ?? Array.Empty<T>();
            }
            else
            {
                var stringData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var errorMessage = $"{response.ReasonPhrase}: {stringData}";

                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new LogicException(errorMessage);
            }
        }
        public static async Task<T?> GetByIdAsync<T>(string baseUri, string extUri, int id)
        {
            using var client = CreateClient(baseUri);
            var response = await client.GetAsync($"{extUri}/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var contentData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<T>(contentData, DeserializerOptions).ConfigureAwait(false);

                return result;
            }
            else
            {
                var stringData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var errorMessage = $"{response.ReasonPhrase}: {stringData}";

                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new LogicException(errorMessage);
            }
        }
        public static async Task<T?> PostAsync<T>(string baseUri, string extUri, T model)
        {
            model.CheckArgument(nameof(model));

            using var client = CreateClient(baseUri);
            var jsonData = JsonSerializer.Serialize(model);
            var contentData = new StringContent(jsonData, Encoding.UTF8, MediaType);
            var response = await client.PostAsync($"{extUri}", contentData).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var resultData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                return await JsonSerializer.DeserializeAsync<T>(resultData, DeserializerOptions).ConfigureAwait(false);
            }
            else
            {
                var errorMessage = $"{response.ReasonPhrase}: { await response.Content.ReadAsStringAsync().ConfigureAwait(false) }";

                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new LogicException(errorMessage);
            }
        }
        public static async Task<T?> PutAsync<T>(string baseUri, string extUri, int id, T model)
        {
            model.CheckArgument(nameof(model));

            using var client = CreateClient(baseUri);
            var jsonData = JsonSerializer.Serialize(model);
            var contentData = new StringContent(jsonData, Encoding.UTF8, MediaType);
            HttpResponseMessage response = await client.PutAsync($"{extUri}/{id}", contentData).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var resultData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                return await JsonSerializer.DeserializeAsync<T>(resultData, DeserializerOptions).ConfigureAwait(false);
            }
            else
            {
                var errorMessage = $"{response.ReasonPhrase}: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}";

                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new LogicException(errorMessage);
            }
        }
        public static async Task DeleteAsync(string baseUri, string extUri, int id)
        {
            using var client = CreateClient(baseUri);
            var response = await client.DeleteAsync($"{extUri}/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode == false)
            {
                var errorMessage = $"{response.ReasonPhrase}: { await response.Content.ReadAsStringAsync().ConfigureAwait(false) }";

                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new LogicException(errorMessage);
            }
        }
    }
}
//MdEnd