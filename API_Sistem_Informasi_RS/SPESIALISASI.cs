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
    
    public partial class SPESIALISASI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SPESIALISASI()
        {
            this.DOKTERs = new HashSet<DOKTER>();
            this.KLINIKs = new HashSet<KLINIK>();
        }
    
        public int ID_SPESIALISASI { get; set; }
        public string JENIS_SPESIALISASI { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOKTER> DOKTERs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KLINIK> KLINIKs { get; set; }
    }
}
