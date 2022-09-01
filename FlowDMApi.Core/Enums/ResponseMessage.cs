using System.ComponentModel;

namespace FlowDMApi.Core.Enums
{
   public enum ResponseMessage
    {
        [Description("işlem sırasında bir sistem hatası oluştu. lütfen tekrar deneyiniz")]
        Error,
        [Description("İşlem Başarı ile Sonuçlandı.")]
        Ok,
        [Description("işlem sırasında bir sistem hatası oluştu. lütfen tekrar deneyiniz")]
        Fatal,
        [Description("Sitedeki çalışmalardan dolayı kapalıyız.")]
        Update,
        [Description("Kontrol Hastası veya Veri Tekrarı")]
        Valid
    }
}
