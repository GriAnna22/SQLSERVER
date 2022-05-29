using System;
using System.Collections.Generic;

#nullable disable

namespace DB2
{
    public partial class StatusAgreement
    {
        public StatusAgreement()
        {
            Agreements = new HashSet<Agreement>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Agreement> Agreements { get; set; }
    }
}
