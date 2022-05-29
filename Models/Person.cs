using System;
using System.Collections.Generic;

#nullable disable

namespace DB2
{
    public partial class Person
    {
        public Person()
        {
            Agreements = new HashSet<Agreement>();
        }

        public int Id { get; set; }
        public int? Inn { get; set; }
        public string Type { get; set; }
        public int Shifer { get; set; }
        public DateTime Data { get; set; }

        public virtual ICollection<Agreement> Agreements { get; set; }
    }
}
