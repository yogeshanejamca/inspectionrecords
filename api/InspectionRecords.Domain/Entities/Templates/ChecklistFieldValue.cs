using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspections.Models.Entities.Templates
{
    public class ChecklistFieldValue
    {
        public int Id { get; set; }

        public int ChecklistId { get; set; }

        public int TemplateFieldId { get; set; }

        public string? Value { get; set; }

        public Checklist Checklist { get; set; } = null!;

        public TemplateField TemplateField { get; set; } = null!;
    }
}
