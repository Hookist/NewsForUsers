namespace NewsForUsers.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Threading.Tasks;

    public partial class NewsForUsersModel : IdentityDbContext<ApplicationUser,
        CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public NewsForUsersModel()
            : base("name=NewsForUsersDb")
        {
        }

        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<SourceToCollection> SourceToCollections { get; set; }
        
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

            modelBuilder.Entity<CustomUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("AspNetUserRoles");

            modelBuilder.Entity<CustomUserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ToTable("AspNetUserLogins");
        }

        public async Task<bool> IsUserHasCollection(int collectionId, int userId)
        {
            Collection collection = await Collections.Where(c => c.Id == collectionId && c.UserId == userId).FirstOrDefaultAsync();
            if (collection == null)
                return false;
            else
                return true;
        }

        public async Task<bool> IsCollectionHasSource(int collectionId, string link)
        {
            var userSourceInDb = await (
                from sc in SourceToCollections
                join c in Collections on sc.CollectionId equals c.Id
                join s in Sources on sc.SourceId equals s.Id
                where c.Id == collectionId && s.Link == link
                select s)
                .FirstOrDefaultAsync();

            if (userSourceInDb == null)
                return false;
            else
                return true;
        }

        public async Task<bool> IsCollectionHasSource(int collectionId, int sourceId)
        {
            var userSourceInDb = await (
                from sc in SourceToCollections
                join c in Collections on sc.CollectionId equals c.Id
                join s in Sources on sc.SourceId equals s.Id
                where c.Id == collectionId && s.Id == sourceId
                select s)
                .FirstOrDefaultAsync();

            if (userSourceInDb == null)
                return false;
            else
                return true;
        }
    }

}
