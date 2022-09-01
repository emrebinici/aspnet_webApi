using Microsoft.AspNetCore.Http;

namespace FlowDMApi.Core.Extentions
{
    public enum ResponseEndEnum
    {
        EndResponse,
        ContinueResponse
    }

    public static class HttpResponseExtensions
    {
        public static void Redirect(this HttpResponse response, string url, ResponseEndEnum responseEnd)
        {
            bool end = (responseEnd == ResponseEndEnum.EndResponse);
            response.Redirect(url, end);
        }
    }
}
