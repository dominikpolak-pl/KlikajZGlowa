using System.Net.Http.Headers;
using KlikajZGlowa.Client.Interfaces;

namespace KlikajZGlowa.Client.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Pobieramy token zapisany w localStorage
            var token = await _localStorage.GetItemAsync<string>("accessToken");

            // Jeśli token istnieje w pamięci przeglądarki, doklejamy go do nagłówka
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}