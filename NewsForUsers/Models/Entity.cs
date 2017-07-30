namespace NewsForUsers.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Entity")]
    public partial class Entity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Text { get; set; }

        [Required]
        [StringLength(500)]
        public string Image { get; set; }

        [Required]
        [StringLength(500)]
        public string PublicationDate { get; set; }

        public int SourceId { get; set; }

        public virtual Source Source { get; set; }
    }
}
