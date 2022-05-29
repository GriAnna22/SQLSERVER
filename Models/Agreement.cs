using System;
using System.Collections.Generic;

#nullable disable

namespace DB2
{
    public partial class Agreement
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public int Number { get; set; }
        public DateTime DataOpen { get; set; }
        public DateTime DataClouse { get; set; }

        public virtual Person Person { get; set; }
        public virtual StatusAgreement StatusAgreement { get; set; }
        public virtual TypeAgreement TypeAgreement { get; set; }
    }
}
