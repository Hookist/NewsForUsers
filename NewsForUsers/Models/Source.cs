namespace NewsForUsers.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Source")]
    public partial class Source
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Source()
        {
            Entities = new HashSet<Entity>();
            SourceToCollections = new HashSet<SourceToCollection>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Link { get; set; }

        public int SourceTypeId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Entity> Entities { get; set; }
        [JsonIgnore]
        public virtual SourceType SourceType { get; set; }

        [JsonIgnore]
        public virtual ICollection<SourceToCollection> SourceToCollections { get; set; }
    }
}
