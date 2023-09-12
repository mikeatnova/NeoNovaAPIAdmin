namespace NeoNovaAPIAdmin.Models
{
    public class AllUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
