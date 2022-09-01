using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FlowDMApi.Core.Enums
{
    public enum ServisCalismaTipiEnum
    {
        [Description("Servis")]
        Servis,
        [Description("Poliklinik Grubu")]
        Poligrup,
        [Description("Servis Grup")]
        ServisGrup     // Yatan Hasta Servis Grubu 
    }
}
