namespace NewsForUsers.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Collection")]
    public partial class Collection
    {
        public Collection()
        {
            SourceToCollections = new HashSet<SourceToCollection>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int UserId { get; set; }

        public virtual ICollection<SourceToCollection> SourceToCollections { get; set; }
    }
}
