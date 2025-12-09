using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class IpAddressService : IIpAddressService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpAddressService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetIpAddress()
        {
            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return string.IsNullOrEmpty(ip) ? "0.0.0.0" : ip;
        }
    }
}
