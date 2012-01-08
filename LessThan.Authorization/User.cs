using System.ComponentModel.DataAnnotations;

namespace LessThan.Authorization
{
    public class User
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string ClaimedIdentifier { get; set; }
    }
}