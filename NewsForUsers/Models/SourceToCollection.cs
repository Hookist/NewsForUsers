namespace NewsForUsers.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SourceToCollection")]
    public partial class SourceToCollection
    {
        public int Id { get; set; }

        public int CollectionId { get; set; }

        public int SourceId { get; set; }
        [JsonIgnore]
        public virtual Collection Collection { get; set; }
        [JsonIgnore]
        public virtual Source Source { get; set; }
    }
}
