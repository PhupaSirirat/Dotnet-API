using System;
using System.Collections.Generic;

namespace ToDoAPI.DTOs
{
    public class User
    {
        public required string IdUser { get; set; }
        public required string Password { get; set; }
        public string? Salt { get; set; }
    }
}