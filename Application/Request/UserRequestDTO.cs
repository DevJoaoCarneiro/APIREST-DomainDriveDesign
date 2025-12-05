using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Request
{
    public class UserRequestDTO
    {
        public string UserId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Mail { get; set; } = string.Empty;

        public AddressRequestDTO UserAddress { get; set; } = new AddressRequestDTO();

        public string password { get; set; } = string.Empty;
    }

    public class AddressRequestDTO
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }
}
