using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Request
{
    public class JadwalRequest
    {
        public int IdJadwal { get; set; }
        public int IdPemeriksaan { get; set; }
        public DateTime TglMulai { get; set; }
        public DateTime TglSelesai { get; set; }
        public string Keperluan { get; set; }
        public string HasilPemeriksaan { get; set; }
        public int Status { get; set; }
    }
}