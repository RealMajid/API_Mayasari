using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Request
{
    public class OrderMedisRequest
    {
        public int IdOrder { get; set; }
        public int IdPemeriksaan { get; set; }
        public int IdPasien { get; set; }
        public int? IdObat { get; set; }
        public int? IdLaborat { get; set; }
        public int Jumlah { get; set; }
        public int JenisOrderMedis { get; set; }
    }
}