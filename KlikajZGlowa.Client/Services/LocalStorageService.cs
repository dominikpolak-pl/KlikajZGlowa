using KlikajZGlowa.Client.Interfaces;
using Microsoft.JSInterop;
using System.Text.Json;

namespace KlikajZGlowa.Client.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _js;

        public LocalStorageService(IJSRuntime js) => _js = js;

        public async ValueTask SetItemAsync<T>(string key, T value)
        {

            var data = typeof(T) == typeof(string) 
                ? value?.ToString() ?? string.Empty 
                : JsonSerializer.Serialize(value);

            await _js.InvokeVoidAsync("localStorage.setItem", key, data);
        }

        public async ValueTask<T?> GetItemAsync<T>(string key)
        {
            var value = await _js.InvokeAsync<string?>("localStorage.getItem", key);
            if (string.IsNullOrWhiteSpace(value)) return default;

            if (typeof(T) == typeof(string))
            {
                return (T)(object)value;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return default;
            }
        }

        public async ValueTask RemoveItemAsync(string key)
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}