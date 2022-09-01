using FlowDMApi.Common.RepositoryBase;
using FlowDMApi.Core.Enums;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.Extentions.Cache;
using FlowDMApi.Core.ResponseBase;
using FlowDMApi.Core.UnitOfWork;
using FlowDMApi.Models;
using System.Collections.Generic;

namespace FlowDMApi.Repository
{
    public class HomeRepository : RepositoryBase
    {
        public HomeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public List<AracKonumGetirViewModel> AracKonumGetir()
        {
            var sql = @"
                     select ar.plate as Plaka, arv.enlem as Enlem, arv.boylam as Boylam from tblArac ar inner join tblArvento arv on ar.arventoNo = ar.arventoNo	where ar.isActive=1;";

            var result = Session.CreateSQLQuery(sql)
                //.DynamicCache(CacheTimeEnum.Day,5)
                .ToTransformedList<AracKonumGetirViewModel>();
            return result;
        }
        
        public List<SevkiyatListesiViewModel> SevkiyatListesi()
        {
            var sql = @"
                     select svk.sevkiyatId AS SevkiyatId, ar.plate AS AracPlaka, 
	                    Concat(p.name, ' ', p.surname) as AracSofor, 
		                    Concat(r.baslangicNoktasi, '-', r.bitisNoktasi) as RotaBilgileri, 
			                    svk.startDate as SevkiyatBaslangicTarihi, 
				                    svk.finishDate as SevkiyatBitisTarihi 
                                        from tblSevkiyat svk 
					                        inner join tblArac ar on svk.aracId=ar.id
						                        inner join tblRotaBilgileri r on r.sevkiyatId = svk.sevkiyatId
							                        inner join tblPersonel p on p.id = svk.soforId 
								                        where svk.isActive=1";

            var result = Session.CreateSQLQuery(sql)
                .DynamicCache(CacheTimeEnum.Hour,5)
                .ToTransformedList<SevkiyatListesiViewModel>();
            return result;
        }
        public GeneralResponse<bool> IlEkle(IlRequestModel request)
        {
            var result = new GeneralResponse<bool>(true);
            result.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
            result.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
            result.Data = true;
            string sql = @"INSERT INTO iller(sehir) VALUES(@sehir); ";
            var sonuc = Session.CreateSQLQuery(sql)
                .SetParameter("sehir", request.sehir)
                .ExecuteUpdateAndGetInsertId();
            if (sonuc <= 0)
            {
                result.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                result.MessageList.Clear();
                result.MessageList.Add(ApiResponseMessage.Error.GetDescription());
                result.Data = false;
            }

            return result;

        }
        public List<IlListesiViewModel> SehirIsimListesi()
        {
            var sql = @"
                     SELECT id, sehir FROM iller;";

            var result = Session.CreateSQLQuery(sql)
                //.DynamicCache(CacheTimeEnum.Day,5)
                .ToTransformedList<IlListesiViewModel>();
            return result;
        }
        public List<UserListViewModel> GetCustomerList()
        {
            var sql = @"
                     SELECT Username, Email FROM Customer;";

            var result = Session.CreateSQLQuery(sql)
                //.DynamicCache(CacheTimeEnum.Day,5)
                .ToTransformedList<UserListViewModel>();
            return result;
        }
        
        public GeneralResponse<bool> AcilHastaGozlemAgriKaydet(AcilHastaGozlemAgriKaydetRequestModel request)
        {
            var result = new GeneralResponse<bool>(true);
            result.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
            result.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
            result.Data = false;
            string sql = @" INSERT INTO p_agridegerleri (
                                    bolge,
                                    deger,
                                    protocol,
                                    sira,
                                    hasta_tipi
                                    )
                                    VALUES
                                    (
                                    @Bolge,
                                    @Deger,
                                    @Protocol,
                                    @Sira,
                                    @Hasta_Tipi
                                    ); ";
            var sonuc = Session.CreateSQLQuery(sql)
                .SetParameter("Bolge", request.Bolge)
                .SetParameter("Deger", request.Deger)
                .SetParameter("Protocol", request.Protocol)
                .SetParameter("Sira", request.Sira)
                .SetParameter("Hasta_Tipi", request.Hasta_Tipi)
                .ExecuteUpdateAndGetInsertId();
            if (sonuc <= 0)
            {
                result.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                result.MessageList.Clear();
                result.MessageList.Add(ApiResponseMessage.Error.GetDescription());
                result.Data = true;
            }

            return result;
        }
        public GeneralResponse<bool> AcilHastaGozlemAgriSil(AcilHastaGozlemAgriSilRequestModel request)
        {
            var result = new GeneralResponse<bool>(true);
            result.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
            result.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
            result.Data = false;
            string sql = @"delete from p_agridegerleri where dosya_id=@DosyaId;";
            var sonuc = Session.CreateSQLQuery(sql)
                .SetParameter("@DosyaId", request.DosyaId)
                .ExecuteUpdate();
            if (sonuc <= 0)
            {
                result.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                result.MessageList.Clear();
                result.MessageList.Add(ApiResponseMessage.Error.GetDescription());
                result.Data = true;
            }

            return result;
        }


        public GeneralResponse<bool> UserEkle(RegisterRequestModel request)
        {
            var result = new GeneralResponse<bool>(true);
            result.ResponseCode = ApiResponseMessage.Ok.GetStringCode();
            result.MessageList.Add(ApiResponseMessage.Ok.GetDescription());
            result.Data = false;
            string sql = @"INSERT INTO users(kullaniciAdi,sifre,kullaniciGorevi,isAdmin) VALUES(@username,@password,@kullaniciGorevi,@isAdmin); ";
            var sonuc = Session.CreateSQLQuery(sql)
                .SetParameter("username", request.username)
                .SetParameter("password", request.password)
                .SetParameter("kullaniciGorevi", "sofor")
                .SetParameter("isAdmin", 1)
                .ExecuteUpdateAndGetInsertId();
            if (sonuc <= 0)
            {
                result.ResponseCode = ApiResponseMessage.Error.GetStringCode();
                result.MessageList.Clear();
                result.MessageList.Add(ApiResponseMessage.Error.GetDescription());
                result.Data = true;
            }
            
            return result;

        }

    }
}
