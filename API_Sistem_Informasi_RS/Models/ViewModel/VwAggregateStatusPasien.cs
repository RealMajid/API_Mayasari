using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.ViewModel
{
    public class VwAggregateStatusPasien
    {
        public string Jenis { get; set; }
        public int Registered { get; set; }
        public int Queuing { get; set; }
        public int Checkup { get; set; }
        public int Closed { get; set; }
    }
}