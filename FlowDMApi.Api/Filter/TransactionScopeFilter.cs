using System;
using FlowDMApi.Api.Controllers;
using FlowDMApi.Core.Enums;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.ResponseBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlowDMApi.Api.Filter
{
    public class TransactionScopeFilter : IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext context)
        {

            var controller = context.Controller as BaseController;
            /*
             * Transaction Aksiyonlar sırası ile takip eder
             * Transaction Hata dönerse rollback yapar.
             * Dönüş tipi eğer ResponseBase ise commit için Success property sine bakar
             */
            var _objectResult = context.Result as ObjectResult;
            if (_objectResult != null && _objectResult.Value is ResponseBase result)
            {
                if (context.Exception != null || !result.Success)
                {
                    try
                    {
                        controller?._context.Rollback();
                        GC.SuppressFinalize(this);
                    }
                    catch { }
                    if (context.Exception != null)
                    {
                        context.ExceptionHandled = true;
                        context.HttpContext.Response.StatusCode = 200;
                        var response = new GeneralResponse(false);
                        response.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                        response.MessageList.Add(context.Exception.Message);
                        context.Result = new JsonResult(response);
                    }
                    return;
                }

                controller?._context.Commit();
                GC.SuppressFinalize(this);
                return;
            }


            if (context.Exception != null)
            {
                try
                {
                    controller?._context.Rollback();
                    GC.SuppressFinalize(this);
                }
                catch { }
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = 200;
                var response = new GeneralResponse(false);
                response.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                response.MessageList.Add(context.Exception.Message);
                context.Result = new JsonResult(response);
                return;
            }

            controller?._context.Commit();
            GC.SuppressFinalize(this);
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
