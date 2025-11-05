using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Client.Models
{
    [PrimaryKey (nameof(Id))]
    [Index(nameof(Name), IsUnique = true)]
    public class ClassExemple
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
    }
}
