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
    
    public partial class VW_MONITOR_LAB
    {
        public int ID_ORDER { get; set; }
        public Nullable<int> ID_PEMERIKSAAN { get; set; }
        public Nullable<int> ID { get; set; }
        public string NAMA { get; set; }
        public Nullable<int> ID_PASIEN { get; set; }
        public string PASIEN { get; set; }
        public int ID_DOKTER { get; set; }
        public string DOKTER { get; set; }
        public int ID_KLINIK { get; set; }
        public string NAMA_KLINIK { get; set; }
        public string STATUS_KASUS { get; set; }
        public System.DateTime TGL_PEMERIKSAAN { get; set; }
    }
}