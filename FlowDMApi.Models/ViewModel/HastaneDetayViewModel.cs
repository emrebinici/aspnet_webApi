using System;

namespace FlowDMApi.Models
{
    [Serializable]
    public class IlListesiViewModel
    {
        public int id { get; set; }
        public string sehir { get; set; }

    }
    public class UserListViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

    }
    public class AracKonumGetirViewModel
    {
        public string Plaka { get; set; }
        public string Enlem { get; set; }
        public string Boylam { get; set; }

    }
    public class SevkiyatListesiViewModel
    {
        public int sevkiyatId { get; set; }
        public string aracPlaka { get; set; }
        public string aracSofor { get; set; }
        public string rotaBilgileri { get; set; }
        public DateTime sevkiyatBaslagicTarihi { get; set; }
        public DateTime sevkiyatBitisTarihi { get; set; }

    }

    public class RegisterViewModel
    {
        public string tokenkey { get; set; }
    }
}