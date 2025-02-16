namespace UserDetailsBackend.Models
{
    public class UserDetails
    {
        // Adding nullable reference types checks if these values are allowed to be null
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
