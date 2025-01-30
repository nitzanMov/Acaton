using Newtonsoft.Json;
using System.Text;

namespace BLL.Helpers

{
    public class HttpHelper
    {
        public class HttpHelperResult<T>
        {
            [JsonIgnore]
            public HttpResponseMessage HttpResponse { get; set; } = new HttpResponseMessage();
            public string? HttpContent { get; set; }
            public T? Data { get; set; }
        }


        protected readonly HttpClient HttpClient;
        protected JsonSerializerSettings Settings;


        public HttpHelper(HttpClient httpClient)
        {
            this.HttpClient = httpClient;

            CreateJsonSettings();
        }


        public string BaseAddress
        {
            get => this.HttpClient?.BaseAddress?.AbsolutePath;

            set
            {
                if (this.HttpClient == null)
                {
                    throw new Exception("HttpClient is null.");
                }
                var uri = new Uri(value);
                if (this.HttpClient.BaseAddress != uri)
                {
                    this.HttpClient.BaseAddress = uri;
                }

            }
        }


        public void SetHeaderKeyAuthentication(string fieldName, string fieldValue)
        {

            if (this.HttpClient.DefaultRequestHeaders.Contains(fieldName))
            {
                return;
            }

            HttpClient.DefaultRequestHeaders.Add(fieldName, fieldValue);
        }


        public async Task<HttpHelperResult<TResponse>> HttpGetAsync<TResponse>(string url, CancellationToken cancelToken = default) where TResponse : class //new()
        {
            var response = new HttpHelperResult<TResponse>
            {
                HttpResponse = await HttpClient.GetAsync(url, cancelToken)
            };

            if (!response.HttpResponse.IsSuccessStatusCode)
            {
                return response;
            }

            response.HttpContent = await response.HttpResponse.Content.ReadAsStringAsync(cancelToken);
            response.Data = JsonConvert.DeserializeObject<TResponse>(response.HttpContent, Settings);

            return response;
        }


        public async Task<HttpHelperResult<TResponse>> HttpPostAsync<TRequest, TResponse>(TRequest request, string url, CancellationToken cancelToken = default) where TResponse : class
        {
            var response = new HttpHelperResult<TResponse>();


            // This is an alternative to PostAsJsonAsync() since it doesn't set the content length in the HTTP header.
            var json = JsonConvert.SerializeObject(request, Settings);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentLength = json.Length;

            response.HttpResponse = await HttpClient.PostAsync(url, httpContent, cancelToken);
            response.HttpContent = await response.HttpResponse.Content.ReadAsStringAsync(cancelToken);
            response.Data = JsonConvert.DeserializeObject<TResponse>(response.HttpContent, Settings);

            return response;
        }


        public async Task<HttpHelperResult<TResponse>> HttpPostAsync<TResponse>(string jsonRequest, string url, CancellationToken cancelToken = default) where TResponse : class
        {
            var response = new HttpHelperResult<TResponse>();

            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            response.HttpResponse = await HttpClient.PostAsync(url, httpContent, cancelToken);
            if (!response.HttpResponse.IsSuccessStatusCode)
            {
                return response;
            }

            response.HttpContent = await response.HttpResponse.Content.ReadAsStringAsync(cancelToken);
            response.Data = JsonConvert.DeserializeObject<TResponse>(response.HttpContent, Settings);

            return response;
        }

        public async Task<HttpHelperResult<TResponse>> HttpPostFormUrlEncodedAsync<TResponse>(Dictionary<string, string> formData, string url, CancellationToken cancelToken = default) where TResponse : class
        {
            var response = new HttpHelperResult<TResponse>();

            var content = new FormUrlEncodedContent(formData);

            response.HttpResponse = await HttpClient.PostAsync(url, content, cancelToken);
            if (!response.HttpResponse.IsSuccessStatusCode)
            {
                return response;
            }
            try
            {
                response.HttpContent = await response.HttpResponse.Content.ReadAsStringAsync(cancelToken);
                response.Data = JsonConvert.DeserializeObject<TResponse>(response.HttpContent);
            }
            catch (Exception ex)
            {

                // Log or handle the exception appropriately
                Console.WriteLine($"Error during deserialization: {ex.Message}");
            }

            return response;
        }

        public async Task<HttpHelperResult<TResponse>> HttpPostFormDataAsync<TResponse>(MultipartFormDataContent formDataContent, string requestUri, CancellationToken cancellationToken = default)
        {
            var response = new HttpHelperResult<TResponse>();

            response.HttpResponse = await HttpClient.PostAsync(requestUri, formDataContent, cancellationToken);

            try
            {
                if (response.HttpResponse.IsSuccessStatusCode)
                {
                    response.HttpContent = await response.HttpResponse.Content.ReadAsStringAsync(cancellationToken);
                    response.Data = JsonConvert.DeserializeObject<TResponse>(response.HttpContent);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error during deserialization: {ex.Message}");
            }
            return response;
        }

        public async Task<HttpHelperResult<TResponse>> HttpPutAsync<TRequest, TResponse>(TRequest request, string url)
        {
            // This is an alternative to PostAsJsonAsync() since it doesn't set the content length in the HTTP header.
            var json = JsonConvert.SerializeObject(request, Settings);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentLength = json.Length;
            var response = new HttpHelperResult<TResponse>
            {
                HttpResponse = await HttpClient.PutAsync(url, httpContent)
            };

            if (!response.HttpResponse.IsSuccessStatusCode)
            {
                return response;
            }

            response.HttpContent = await response.HttpResponse.Content.ReadAsStringAsync();
            response.Data = JsonConvert.DeserializeObject<TResponse>(response.HttpContent, Settings);

            return response;
        }


        private void CreateJsonSettings()
        {
            Settings = new JsonSerializerSettings
            {
                Error = HandleDeserializationError,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
        }

        private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
