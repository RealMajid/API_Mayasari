using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Request
{
    public class RegisterKasusRequest
    {
        public string Nama { get; set; }
        public string Alamat { get; set; }
        public DateTime TglLahir { get; set; }
        public string TempatLahir { get; set; }
        public string Email { get; set; }
        public string Telp { get; set; }
        public string JenisKelamin { get; set; }
        public string Keluhan { get; set; }
        public int Umur { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int TinggiBadan { get; set; }
        public int BeratBadan { get; set; }
        public int TensiDarah { get; set; }
        public int Klinik { get; set; }
        public string JenisPasien { get; set; }
        public int? IdPasien { get; set; }
        public int? IdUser { get; set; }
    }
}