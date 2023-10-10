using System;
using System.Collections.Generic;

namespace ToDoAPI.DTOs
{
    public class Login
    {
        public required string IdUser { get; set; }
        public required string Password { get; set; }
    }
}