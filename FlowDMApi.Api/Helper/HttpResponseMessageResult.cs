using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace FlowDMApi.Api.Helper
{
    public class HttpResponseMessageResult : IActionResult
    {
        public readonly HttpResponseMessage ResponseMessage;

        public HttpResponseMessageResult(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage; // could add throw if null
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)ResponseMessage.StatusCode;

            foreach (var header in ResponseMessage.Headers)
            {
                context.HttpContext.Response.Headers.TryAdd(header.Key, new StringValues(header.Value.ToArray()));
            }

            using (var stream = await ResponseMessage.Content.ReadAsStreamAsync())
            {
                await stream.CopyToAsync(context.HttpContext.Response.Body);
                await context.HttpContext.Response.Body.FlushAsync();
            }

        }
    }
}
