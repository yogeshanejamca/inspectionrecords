using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspections.Models.Entities.Templates
{
    public class Checklist
    {
        public int Id { get; set; }

        public int InspectionId { get; set; }

        public int TemplateVersionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Inspection Inspection { get; set; } = null!;

        public TemplateVersion TemplateVersion { get; set; } = null!;

        public ICollection<ChecklistFieldValue> FieldValues { get; set; }
            = new List<ChecklistFieldValue>();
    }
}
