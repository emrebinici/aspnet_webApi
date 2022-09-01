using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.ResponseBase;

namespace FlowDMApi.Api.Helper
{
    public static class ActionResultHelper
    {
        public static Task<HttpResponseMessage> HttpResponseResult(this GeneralResponse result)
        {
            string contentresult = SerializerHelper.Serialize(result);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(contentresult, Encoding.UTF8, "application/json")

            });
        }
        public static Task<HttpResponseMessage> HttpResponseResult<T>(this GeneralResponse<T> result)
        {
            string contentresult = SerializerHelper.Serialize(result);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(contentresult, Encoding.UTF8, "application/json")

            });
        }
        public static Task<HttpResponseMessage> HttpResponseResult<T>(this GeneralListResponse<T> result)
        {
            string contentresult = SerializerHelper.Serialize(result);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(contentresult, Encoding.UTF8, "application/json")

            });
        }
        public static Task<HttpResponseMessage> HttpResponseResult<T>(this GeneralPagedListResponse<T> result)
        {
            string contentresult = SerializerHelper.Serialize(result);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(contentresult, Encoding.UTF8, "application/json")

            });
        }
    }
}
