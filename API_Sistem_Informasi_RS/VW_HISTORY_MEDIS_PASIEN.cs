//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API_Sistem_Informasi_RS
{
    using System;
    using System.Collections.Generic;
    
    public partial class VW_HISTORY_MEDIS_PASIEN
    {
        public string REG_NO { get; set; }
        public int ID_PASIEN { get; set; }
        public string PASIEN { get; set; }
        public Nullable<System.DateTime> TGL_MASUK { get; set; }
        public string KELUHAN { get; set; }
        public string DIAGNOSA { get; set; }
        public string NAMA_KLINIK { get; set; }
        public string DOKTER { get; set; }
        public string RUANGAN { get; set; }
        public string STATUS_TUNGGU { get; set; }
        public Nullable<int> ID_PEMERIKSAAN { get; set; }
        public string JENIS_SPESIALISASI { get; set; }
        public Nullable<int> ID_DOKTER { get; set; }
        public int ID_KLINIK { get; set; }
    }
}
