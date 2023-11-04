using System.ComponentModel.DataAnnotations;

namespace GatePass_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
    }
}
