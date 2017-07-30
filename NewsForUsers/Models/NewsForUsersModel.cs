namespace NewsForUsers.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;

    public partial class NewsForUsersModel : IdentityDbContext<ApplicationUser>
    {
        public NewsForUsersModel()
            : base("name=NewsForUsersDb")
        {
        }

        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<SourceToCollection> SourceToCollections { get; set; }
        public virtual DbSet<SourceType> SourceTypes { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collection>()
                .HasMany(e => e.SourceToCollections)
                .WithRequired(e => e.Collection)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Source>()
                .HasMany(e => e.Entities)
                .WithRequired(e => e.Source)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Source>()
                .HasMany(e => e.SourceToCollections)
                .WithRequired(e => e.Source)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SourceType>()
                .HasMany(e => e.Sources)
                .WithRequired(e => e.SourceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("AspNetUserRoles");

            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ToTable("AspNetUserLogins");
        }
    }
}
