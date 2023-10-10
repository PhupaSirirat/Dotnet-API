
namespace ToDoAPI.DTOs
{
    public class Password
    {
        public required string password { get; set; }
        public required string salt { get; set; }
    }
}