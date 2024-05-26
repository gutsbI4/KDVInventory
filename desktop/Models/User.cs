using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string? Image { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string IsActive { get; set; }
        public bool IsArchive { get; set; }
        public DateTime? FirstEntry { get; set; }
        public DateTime? LastAction { get; set; }

    }
}
