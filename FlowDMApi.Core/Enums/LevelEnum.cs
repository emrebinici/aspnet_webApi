namespace FlowDMApi.Core.Enums
{
    public enum Level
    {
        // Tüm mesajların loglandığı seviyedir.
        ALL,
        // Developement aşamasına yönelik loglama seviyesidir.
        DEBUG,
        // Uygulama içerisindeki bilgileri loglayabildiğiniz seviyedir.
        INFO,
        // Hata olmayan fakat önemli bir durumun oluştuğunu belirtebileceğimiz seviye.
        WARN,
        //Hata durumunu belirten seviye. Sistem hala çalışır haldedir.
        ERROR,
        //Uygulamanın sonlanacağını, faaliyet gösteremeyeceğini belirten mesajlar için kullanılacak seviyedir.
        FATAL,
        //Hiç bir mesajın loglanmadığı seviyedir.
        OFF

    }
}
