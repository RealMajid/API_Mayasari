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
    
    public partial class KLINIK
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KLINIK()
        {
            this.REGISTER_KASUS = new HashSet<REGISTER_KASUS>();
        }
    
        public int ID_KLINIK { get; set; }
        public string NAMA_KLINIK { get; set; }
        public Nullable<int> ID_SPESIALISASI { get; set; }
    
        public virtual SPESIALISASI SPESIALISASI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REGISTER_KASUS> REGISTER_KASUS { get; set; }
    }
}
