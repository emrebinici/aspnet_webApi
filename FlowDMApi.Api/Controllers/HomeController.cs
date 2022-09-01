
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowDMApi.Api.Filter;
using FlowDMApi.Core.Enums;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.ResponseBase;
using FlowDMApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowDMApi.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : BaseController
    {

        //[SkipAuthorize, HttpPost]
        //public GeneralListResponse<SevkiyatListesiViewModel> SevkiyatListesi()
        //{
        //    var result = _context.HomeRepository.SevkiyatListesi();
        //    var response = new GeneralListResponse<SevkiyatListesiViewModel>(true);
        //    response.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
        //    response.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
        //    response.Data = result;
        //    return response;
        //}

        [SkipAuthorize, HttpPost]
        public GeneralListResponse<IlListesiViewModel> SehirIsimListesi()
        {
            var result = _context.HomeRepository.SehirIsimListesi();
            var response = new GeneralListResponse<IlListesiViewModel>(true);
            response.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
            response.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
            response.Data = result;
            return response;
        }
        [SkipAuthorize, HttpGet]
        public GeneralListResponse<UserListViewModel> GetCustomerList()
        {
            var result = _context.HomeRepository.GetCustomerList();
            var response = new GeneralListResponse<UserListViewModel>(true);
            response.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
            response.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
            response.Data = result;
            return response;
        }

        [SkipAuthorize, HttpPost]
        public GeneralResponse<bool> IlEkle([FromForm]IlRequestModel request)
        {
            var result = _context.HomeRepository.IlEkle(request);
            return result;
        }
        //[SkipAuthorize, HttpPost]
        //public GeneralListResponse<AracKonumGetirViewModel> AracKonumGetir()
        //{
        //    var result = _context.HomeRepository.AracKonumGetir();
        //    var response = new GeneralListResponse<AracKonumGetirViewModel>(true);
        //    response.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
        //    response.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
        //    response.Data = result;
        //    return response;
        //}

        //[SkipAuthorize, HttpPost]
        //public GeneralResponse<bool> HastaGozlemAgriKaydet(AcilHastaGozlemAgriKaydetRequestModel request)
        //{
        //    var result = _context.HomeRepository.AcilHastaGozlemAgriKaydet(request);
        //    return result;
        //}

        //[SkipAuthorize, HttpPost]
        //public GeneralResponse<bool> HastaGozlemAgriSil(AcilHastaGozlemAgriSilRequestModel request)
        //{
        //    var result = _context.HomeRepository.AcilHastaGozlemAgriSil(request);
        //    return result;
        //}

        //[SkipAuthorize, HttpPost]
        //public GeneralResponse<bool> UserEkle([FromForm] RegisterRequestModel request)
        //{
        //    var result = _context.HomeRepository.UserEkle(request);
        //    return result;
        //}
    }
}
