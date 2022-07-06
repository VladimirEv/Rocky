using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class Well
    {
        public int Id { get; set; }
        public string WellheadRewinding { get; set; }
        public string WellDepth { get; set; }
    }
}
