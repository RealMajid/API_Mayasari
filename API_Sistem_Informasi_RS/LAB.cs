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
    
    public partial class LAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LAB()
        {
            this.ORDER_MEDIS = new HashSet<ORDER_MEDIS>();
        }
    
        public int ID_LABORAT { get; set; }
        public int ID_KLINIK { get; set; }
        public string NAMA_TES { get; set; }
        public string GUNA_TES { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDER_MEDIS> ORDER_MEDIS { get; set; }
    }
}
