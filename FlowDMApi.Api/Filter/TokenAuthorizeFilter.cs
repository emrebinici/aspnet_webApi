using System;
using System.Collections.Generic;
using System.Linq;
using FlowDMApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FlowDMApi.Core.Enums;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.ResponseBase;

namespace FlowDMApi.Api.Filter
{
    public class TokenAuthorizeFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers == null)
            {
                var response = new ObjectResult(new GeneralResponse
                {
                    Success = false,
                    ResponseCode = ApiResponseMessage.AcceptContentType.GetStringCode(),
                    MessageList = new List<string>
                    {
                        ApiResponseMessage.AcceptContentType.GetDescription()
                    }
                });
                response.ContentTypes.Add("application/json");
                response.StatusCode = 200;
                context.Result = response;
                return;
            }

            var accept = context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Accept");
            if (!string.IsNullOrEmpty(accept.Value) && !accept.Value.ToString().ToLower().Contains("application/json"))
            {
                var response = new ObjectResult(new GeneralResponse
                {
                    Success = false,
                    ResponseCode = ApiResponseMessage.AcceptContentType.GetStringCode(),
                    MessageList = new List<string>
                    {
                        ApiResponseMessage.AcceptContentType.GetDescription()
                    }
                });
                response.ContentTypes.Add("application/json");
                response.StatusCode = 200;
                context.Result = response;
                return;
            }

            if (SkipAuthorizeCheck(context))
            {
                return;
            }

            var token = context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Authorization");
            if (string.IsNullOrEmpty(token.Value))
            {
                var response = new ObjectResult(new GeneralResponse
                {
                    Success = false,
                    ResponseCode = ApiResponseMessage.UnAuthorizedToken.GetStringCode(),
                    MessageList = new List<string>
                    {
                        ApiResponseMessage.UnAuthorizedToken.GetDescription()
                    }
                });
                response.ContentTypes.Add("application/json");
                response.StatusCode = 200;
                context.Result = response;
                return;
            }
            var controller = context.Controller as BaseController;
            var tokendurum = controller?._context.TokenRepository.GetTokenModelByToken(token.Value);
            if (tokendurum == null || DateTime.Now > tokendurum.Expiration)
            {
                var response = new ObjectResult(new GeneralResponse
                {
                    Success = false,
                    ResponseCode = ApiResponseMessage.UnAuthorizedToken.GetStringCode(),
                    MessageList = new List<string>
                    {
                        ApiResponseMessage.UnAuthorizedToken.GetDescription()
                    }
                });
                response.ContentTypes.Add("application/json");
                response.StatusCode = 200;
                context.Result = response;
                return;
            }
        }

        private bool SkipAuthorizeCheck(ActionExecutingContext context)
        {
            var filter = context.ActionDescriptor.EndpointMetadata.OfType<SkipAuthorize>();

            if (filter.Any())
            {
                return true;
            }
            return false;
        }
    }
    public class SkipAuthorize : ActionFilterAttribute
    {
    }
}
