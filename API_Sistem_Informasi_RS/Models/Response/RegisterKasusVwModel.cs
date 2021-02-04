using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Response
{
    public class RegisterKasusVwModel
    {
        public string NoKasus { get; set; }
        public string Pasien { get; set; }
        public DateTime TglMasuk { get; set; }
        public string Keluhan { get; set; }
        public string Klinik { get; set; }
        public string Dokter { get; set; }
        public string Status { get; set; }
    }
}