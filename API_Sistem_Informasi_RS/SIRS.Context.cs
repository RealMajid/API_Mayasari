﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class mayasariEntities : DbContext
    {
        public mayasariEntities()
            : base("name=mayasariEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DOKTER> DOKTERs { get; set; }
        public virtual DbSet<KASU> KASUS { get; set; }
        public virtual DbSet<KASUS_STAT> KASUS_STAT { get; set; }
        public virtual DbSet<KLINIK> KLINIKs { get; set; }
        public virtual DbSet<ORDER_MEDIS> ORDER_MEDIS { get; set; }
        public virtual DbSet<PASIEN> PASIENs { get; set; }
        public virtual DbSet<PEMERIKSAAN> PEMERIKSAANs { get; set; }
        public virtual DbSet<REGISTER_KASUS> REGISTER_KASUS { get; set; }
        public virtual DbSet<REKAM_MEDIS> REKAM_MEDIS { get; set; }
        public virtual DbSet<ROLE> ROLES { get; set; }
        public virtual DbSet<SPESIALISASI> SPESIALISASIs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<T_KASUS> T_KASUS { get; set; }
        public virtual DbSet<USER> USERS { get; set; }
        public virtual DbSet<VW_REGISTER_KASUS> VW_REGISTER_KASUS { get; set; }
        public virtual DbSet<VW_ALOKASI_DOKTER> VW_ALOKASI_DOKTER { get; set; }
        public virtual DbSet<VW_HISTORY_MEDIS_PASIEN> VW_HISTORY_MEDIS_PASIEN { get; set; }
        public virtual DbSet<LAB> LABs { get; set; }
        public virtual DbSet<OBAT> OBATs { get; set; }
        public virtual DbSet<VW_ORDER_MEDIS> VW_ORDER_MEDIS { get; set; }
        public virtual DbSet<VW_MONITOR_LAB> VW_MONITOR_LAB { get; set; }
        public virtual DbSet<JADWAL> JADWALs { get; set; }
    }
}
