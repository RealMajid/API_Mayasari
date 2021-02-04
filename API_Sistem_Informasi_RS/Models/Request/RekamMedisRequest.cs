using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Request
{
    public class RekamMedisRequest
    {
        public string RegNo { get; set; }
        public int IdPemeriksaan { get; set; }
        public int IdRekamMedis { get; set; }
        public string Gejala { get; set; }
        public string Diagnosa { get; set; }
        public string Tindakan { get; set; }
        public string HasilTestLab { get; set; }
    }
}