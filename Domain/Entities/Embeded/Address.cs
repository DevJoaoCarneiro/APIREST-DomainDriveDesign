using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Embeded
{
    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get;  set; }

    }
}
