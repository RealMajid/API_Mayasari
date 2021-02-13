using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Report
{
    public class RptDetailPemeriksaanPasien
    {
        public string REG_NO { get; set; }
        public string PASIEN { get; set; }
        public string DOKTER { get; set; }
        public string NAMA_KLINIK { get; set; }
        public DateTime TGL_MASUK { get; set; }
        public string RUANGAN { get; set; }
        public string KELUHAN { get; set; }
        public string GEJALA { get; set; }
        public string DIAGNOSA { get; set; }
        public string TINDAKAN { get; set; }
        public string HASIL_TEST_LAB { get; set; }
    }
}