using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.ViewModels
{
    public class JWTPayloadViewModel
    {
        public object exp { get; set; }

        public int Id { get; set; }
        public string Username { get; set; }
    }
}