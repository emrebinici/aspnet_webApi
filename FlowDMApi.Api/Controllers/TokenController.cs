using System;
using FlowDMApi.Api.Filter;
using FlowDMApi.Core.Enums;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.ResponseBase;
using FlowDMApi.Models.Token;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FlowDMApi.Api.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : BaseController
    {
        [SkipAuthorize, HttpPost, Route("tokenal")]
        public GeneralResponse<TokenModel> TokenAl(TokenRequest request)
        {
            var response = new GeneralResponse<TokenModel>(true);
            var kullanici = _context.TokenRepository.GetKullanici(request.KullaniciAdi, request.KullaniciSifre);
            if (kullanici == null)
            {
                response.ResponseCode = ApiResponseMessage.UnAuthorizedPasswordUser.GetStringCode();
                response.MessageList.Add(ApiResponseMessage.UnAuthorizedPasswordUser.GetDescription());
                return response;
            }

            var token = _context.TokenRepository.GetTokenByKullaniciId(kullanici.KullaniciId);
            if (token == null)
            {
                var tokenString = _context.TokenRepository.TokenUret();
                token = new TokenModel()
                {
                    Token = tokenString,
                    Expiration = DateTime.Now.AddDays(7)
                };
                var s = _context.TokenRepository.SaveTokenByKullaniciId(kullanici.KullaniciId, token);
                if (!s)
                {
                    response.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                    response.MessageList.Add(ApiResponseMessage.Error.GetDescription());
                    return response;
                }
            }
            if (DateTime.Now > token.Expiration)
            {
                var tokenString = _context.TokenRepository.TokenUret();
                token = new TokenModel()
                {
                    Token = tokenString,
                    Expiration = DateTime.Now.AddDays(7)
                };
                var s = _context.TokenRepository.UpdateTokenByKullaniciId(kullanici.KullaniciId, token);
                if (!s)
                {
                    response.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                    response.MessageList.Add(ApiResponseMessage.Error.GetDescription());
                    return response;
                }
            }
            response.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
            response.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
            response.Data = token;
            return response;
        }

        [SkipAuthorize, Route("handleerrors")]
        private GeneralResponse HandleErrors()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;
            var response = new GeneralResponse(false);
            response.ResponseCode = ApiResponseMessage.Fatal.GetStringCode();
            response.MessageList.Add(ApiResponseMessage.Fatal.GetDescription());
            return response;
        }
    }
}




