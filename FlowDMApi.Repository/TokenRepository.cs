using System;
using FlowDMApi.Common.RepositoryBase;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.Extentions.Cache;
using FlowDMApi.Core.UnitOfWork;
using FlowDMApi.Models.Token;

namespace FlowDMApi.Repository
{
    public class TokenRepository : RepositoryBase
    {
        public TokenRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public KullaniciModel GetKullanici(string kullaniciAdi, string kullaniciSifre)
        {
            return Session.CreateSQLQuery(@"select id as KullaniciId,kullaniciadi as KullaniciAdi,aktif as Aktif from prestij.altivaapikullanici where kullaniciadi = @kullaniciadi and kullanicisifre = @sifre and aktif = 1 limit 1")
                .SetParameter("kullaniciadi", kullaniciAdi)
                .SetParameter("sifre", kullaniciSifre)
                .DynamicCache(CacheTimeEnum.Day)
                .CacheTime(1)
                .TransformedUniqueResult<KullaniciModel>();
        }

        public TokenModel GetTokenModelByToken(string token)
        {
            var tokenModel = Redis.Get<TokenModel>($"TokenInfo:{token}");
            if (tokenModel == null)
            {
                tokenModel = GetToken(token);
            }
            if (tokenModel != null)
            {
                Redis.Set($"TokenInfo:{token}", tokenModel, tokenModel.Expiration);
            }
            return tokenModel;
        }
        public TokenModel GetTokenByKullaniciId(long kullaniciId)
        {
            return Session.CreateSQLQuery(@"select token as Token,expiration as Expiration from prestij.altivaapitoken where kullaniciid = @kullaniciid limit 1")
                .SetParameter("kullaniciid", kullaniciId)
                .DynamicCache(CacheTimeEnum.Day)
                .CacheTime(1)
                .TransformedUniqueResult<TokenModel>();
        }
        public TokenModel GetToken(string tokenString)
        {
            return Session.CreateSQLQuery(@"select token as Token,expiration as Expiration from prestij.altivaapitoken where token = @token limit 1")
                .SetParameter("token", tokenString)
                .DynamicCache(CacheTimeEnum.Day)
                .CacheTime(1)
                .TransformedUniqueResult<TokenModel>();
        }
        public bool SaveTokenByKullaniciId(long kullaniciId, TokenModel token)
        {
            return Session.CreateSQLQuery(@"insert into prestij.altivaapitoken set token=@token, expiration=@expiration, kullaniciid=@kullaniciid")
                .SetParameter("kullaniciid", kullaniciId)
                .SetParameter("token", token.Token)
                .SetParameter("expiration", token.Expiration)
                .ExecuteUpdate() > 0;
        }
        public bool UpdateTokenByKullaniciId(long kullaniciId, TokenModel token)
        {
            return Session.CreateSQLQuery(@"update prestij.altivaapitoken set token=@token, expiration=@expiration where kullaniciid=@kullaniciid")
                .SetParameter("kullaniciid", kullaniciId)
                .SetParameter("token", token.Token)
                .SetParameter("expiration", token.Expiration)
                .ExecuteUpdate() > 0;
        }
        public string TokenUret()
        {
            var token = ("System" + Guid.NewGuid()).ToMd5();
            return token;
        }

        public void deneme()
        {
            Session.CreateSQLQuery("insert into denemehalil(deneme) values('deneme')").ExecuteUpdateAndGetInsertId();
        }
    }
}
