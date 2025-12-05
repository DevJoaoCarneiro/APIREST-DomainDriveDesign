using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Embeded
{
    public class Address
    {
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }

        private Address() { }

        public Address(string street, string number, string city, string state, string zipCode)
        {
            Street = street;
            Number = number;
            City = city;
            State = state;
            ZipCode = zipCode;
        }
    }
}
