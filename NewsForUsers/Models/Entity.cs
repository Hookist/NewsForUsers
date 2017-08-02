namespace NewsForUsers.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Entity")]
    public partial class Entity
    {
        public int Id { get; set; }

        [StringLength(300)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Text { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(300)]
        public string Link { get; set; }

        [StringLength(500)]
        public string PublicationDate { get; set; }

        [JsonIgnore]
        public int SourceId { get; set; }
        [JsonIgnore]
        public virtual Source Source { get; set; }
    }
}
