using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspections.Models.Entities.Templates
{
    public class TemplateVersion
    {
        public int Id { get; set; }

        public int TemplateId { get; set; }

        public int VersionNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public Template Template { get; set; } = null!;

        public ICollection<TemplateField> Fields { get; set; }
            = new List<TemplateField>();
    }
}
