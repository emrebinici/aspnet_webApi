using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowDMApi.Models
{
    public class AcilHastaGozlemAgriKaydetRequestModel
    {
        public string Bolge { get; set; }
        public int Deger { get; set; }
        public long Protocol { get; set; }
        public long Sira { get; set; }
        public string Hasta_Tipi { get; set; }
    }


    public class RegisterRequestModel
    {

        public string username { get; set; }
        public string  password { get; set; }
    }
    public class IlRequestModel
    {

        public string sehir { get; set; }
    }
}
