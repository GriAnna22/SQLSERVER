using System;
using System.Collections.Generic;

#nullable disable

namespace DB2
{
    public partial class TypeAgreement
    {
        public TypeAgreement()
        {
            Agreements = new HashSet<Agreement>();
        }

        public int Id { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Agreement> Agreements { get; set; }
    }
}
