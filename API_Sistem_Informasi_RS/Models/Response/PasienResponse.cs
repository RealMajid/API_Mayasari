using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Response
{
    public class PasienResponse
    {
        public int ID_PASIEN { get; set; }
        public int? ID_USER { get; set; }
        public string NAMA { get; set; }
        public DateTime TGL_LAHIR { get; set; }
        public string ALAMAT { get; set; }
        public string JENIS_KELAMIN { get; set; }
        public string TELP { get; set; }
        public string TEMPAT_LAHIR { get; set; }
        public string EMAIL { get; set; }
    }
}