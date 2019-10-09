namespace Logman.Common.DomainObjects
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ActivationKey { get; set; }
        public bool Enabled { get; set; }
        public string PasswordSalt { get; set; }
        public Roles CurrentRole { get; set; }
    }
}