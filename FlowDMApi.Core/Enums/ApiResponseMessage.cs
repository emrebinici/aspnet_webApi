using System.ComponentModel;
using FlowDMApi.Core.Attributes;

namespace FlowDMApi.Core.Enums
{
    public enum ApiResponseMessage
    {
        [Description("İşlem Sırasında Teknik Sorunlar Oluşmuştur. Lütfen Daha Sonra Tekrar Deneyiniz !")]
        [ResponseCode("E0001")]
        Error,
        [Description("İşlem Başarı İle Tamamlanmıştır !")]
        [ResponseCode("S0000")]
        Ok,
        [ResponseCode("F0001")]
        [Description("İşlem Sırasında Teknik Sorunlar Oluşmuştur. Lütfen Daha Sonra Tekrar Deneyiniz !")]
        Fatal,
        [Description("Servis Teknik Bakımdadır ve Hizmete Kapalıdır.")]
        [ResponseCode("U0001")]
        Update,
        [ResponseCode("V0001")]
        [Description("Verilerde Hata Olduğu Gözlenmiştir. Veri Kontrolü Yapıldıktan Sonra Tekrar Gönderiniz !")]
        Valid,
        [ResponseCode("AE0001")]
        [Description("Gönderdiğiniz Token Geçersizdir. Bu Nedenle Tekrar Token Almanız Gerekmektedir !")]
        UnAuthorizedToken,
        [ResponseCode("AE0002")]
        [Description("Geçersiz Kullanıcı Adı ve Şifre Durumu Gözlenmiştir !")]
        UnAuthorizedPasswordUser,
        [ResponseCode("G0001")]
        [Description("Geçersiz İstek Yolu Gözlenmiştir !")]
        NotFound,
        [ResponseCode("CT0001")]
        [Description("Gönderilen Accept Bilgisi Yanlıştır. Application/json Olması Gerekmektedir.")]
        AcceptContentType

    }
}
