using System.Text;
using System.Text.Json;

namespace StargateApi.Tests.TestUtilities;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PostAsJsonAsync<TContent>(
        this HttpClient client,
        string requestUri,
        TContent content)
        where TContent : class =>
        client.SendAsJsonAsync(
            method: HttpMethod.Post,
            requestUri: requestUri,
            content: content);

    public static Task<HttpResponseMessage> PutAsJsonAsync<TContent>(
        this HttpClient client,
        string requestUri,
        TContent content)
        where TContent : class => 
        client.SendAsJsonAsync(
            method: HttpMethod.Put,
            requestUri: requestUri,
            content: content);

    private static Task<HttpResponseMessage> SendAsJsonAsync<TContent>(
        this HttpClient client,
        HttpMethod method,
        string requestUri,
        TContent content)
        where TContent : class
    {
        var request = new HttpRequestMessage(
            method: method,
            requestUri: requestUri)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(content),
                encoding: Encoding.UTF8,
                mediaType: "application/json")
        };
        return client.SendAsync(
            request: request);
    }
}
