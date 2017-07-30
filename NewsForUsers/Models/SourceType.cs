namespace NewsForUsers.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SourceType")]
    public partial class SourceType
    {
        public SourceType()
        {
            Sources = new HashSet<Source>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Source> Sources { get; set; }
    }
}
