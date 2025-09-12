using System.Text.Json;

namespace StargateApi.Tests.TestUtilities;

public static class HttpResponseMessageExtensions
{
    private static JsonSerializerOptions WEB_SERIALIZER_OPTIONS = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public static async Task<TContent> DeserializeResponseContentAsync<TContent>(
        this HttpResponseMessage response)
        where TContent : class
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<TContent>(
            json: responseContent,
            options: WEB_SERIALIZER_OPTIONS);
        Assert.IsNotNull(content);
        return content;
    }
}
