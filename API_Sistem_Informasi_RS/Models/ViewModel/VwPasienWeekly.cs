using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.ViewModel
{
    public class VwPasienWeekly
    {
        public int IdHari { get; set; }
        public string Hari { get; set; }
        public int Registered { get; set; }
        public int Queuing { get; set; }
        public int Checkup { get; set; }
        public int Closed { get; set; }
    }
}