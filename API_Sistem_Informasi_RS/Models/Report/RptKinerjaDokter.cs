using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Report
{
    public class RptKinerjaDokter
    {
        public string DOKTER { get; set; }
        public string JENIS_SPESIALISASI { get; set; }
        public int ID_PEMERIKSAAN { get; set; }
        public DateTime TGL_MASUK { get; set; }
        public DateTime TGL_KELUAR { get; set; }
        public string LAMA_PEMERIKSAAN { get; set; }
        public string KINERJA { get; set; }
    }
}