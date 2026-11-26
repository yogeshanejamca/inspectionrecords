using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspections.Models.DTOs.Template
{
    public class ChecklistDto
    {
        public int Id { get; set; }

        public int InspectionId { get; set; }

        public int TemplateId { get; set; }

        public string TemplateName { get; set; } = string.Empty;

        public List<ChecklistFieldValueDto> Fields { get; set; }
            = new();
    }
}
