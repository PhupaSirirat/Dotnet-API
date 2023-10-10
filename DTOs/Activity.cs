using System;
using System.Collections.Generic;

namespace ToDoAPI.DTOs
{
    public class Activity
    {
        public required string Name { get; set; }
        public required DateTime When { get; set; }
        public string IdUser { get; set; }
    }
}