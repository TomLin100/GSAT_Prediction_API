﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GSATPrediction.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GSAT_Prediction_2017Entities : DbContext
    {
        public GSAT_Prediction_2017Entities()
            : base("name=GSAT_Prediction_2017Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CG> CGs { get; set; }
        public virtual DbSet<D> D { get; set; }
        public virtual DbSet<DC> DCs { get; set; }
        public virtual DbSet<NewScoreData> NewScoreDatas { get; set; }
        public virtual DbSet<OldScoreData> OldScoreDatas { get; set; }
        public virtual DbSet<T> T { get; set; }
    }
}
