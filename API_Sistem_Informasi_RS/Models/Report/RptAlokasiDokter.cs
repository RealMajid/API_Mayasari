using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Report
{
    public class RptAlokasiDokter
    {
        public string Dokter { get; set; }
        public string Spesialisasi { get; set; }
        public int JumlahPemeriksaan { get; set; }
        public int Queuing { get; set; }
        public int Checkup { get; set; }
        public int Closed { get; set; }
    }
}