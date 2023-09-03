
using System.Text;

public class ProxyMiddleware
{
    const string ProxyUrl = "/proxy";

    public ProxyMiddleware(RequestDelegate _, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        m_httpClient = httpClientFactory.CreateClient();
        m_username = configuration["mbauth:username"];
        m_password = configuration["mbauth:password"];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(ProxyUrl))
        {
            var request = CreateHttpRequest(context);
            using var response = await m_httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, context.RequestAborted);

            await CopyResponseToContext(context, response);
        }
    }

    private static Uri GetUrl(HttpContext context)
    {
        var url = context.Request.Path.ToUriComponent();
        url = url.Substring(ProxyUrl.Length + 1);
        return new(context.Request.Scheme + "://" + url + context.Request.QueryString);
    }

    private HttpRequestMessage CreateHttpRequest(HttpContext context)
    {
        var url = GetUrl(context);
        var method = context.Request.Method!;

        HttpRequestMessage request = new()
        {
            Method = GetMethod(method),
            RequestUri = url,
        };

        if (!HttpMethods.IsGet(method) &&
           !HttpMethods.IsHead(method) &&
           !HttpMethods.IsDelete(method) &&
           !HttpMethods.IsTrace(method))
        {
            request.Content = new StreamContent(context.Request.Body);
            request.Content.Headers.TryAddWithoutValidation("Content-Type", context.Request.ContentType);
        }

        foreach (var header in context.Request.Headers)
        {
            if (header.Key == "Host")
                continue;

            if (header.Key == "Authorization" && url == new Uri("https://apiauth.macrobondfinancial.com/mbauth/connect/token"))
            {
                string authorization = header.Value.ToString();
                authorization = authorization.Substring("Basic ".Length);
                authorization = Encoding.UTF8.GetString(Convert.FromBase64String(authorization));
                if (authorization == "test:test")
                {
                    authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(m_username + ":" + m_password));
                    request.Headers.TryAddWithoutValidation("Authorization", authorization);
                    continue;
                }
            }

            request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        request.Headers.Host = url.Host;

        return request;
    }

    private static async Task CopyResponseToContext(HttpContext context, HttpResponseMessage response)
    {
        context.Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        context.Response.Headers.Remove("transfer-encoding");

        await response.Content.CopyToAsync(context.Response.Body);
    }

    private static HttpMethod GetMethod(string method)
    {
        if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
        if (HttpMethods.IsGet(method)) return HttpMethod.Get;
        if (HttpMethods.IsHead(method)) return HttpMethod.Head;
        if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
        if (HttpMethods.IsPost(method)) return HttpMethod.Post;
        if (HttpMethods.IsPut(method)) return HttpMethod.Put;
        if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
        return new HttpMethod(method);
    }

    private readonly HttpClient m_httpClient;
    private readonly string? m_username;
    private readonly string? m_password;
}