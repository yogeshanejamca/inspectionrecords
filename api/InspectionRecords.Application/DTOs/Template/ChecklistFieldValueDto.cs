using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspections.Models.DTOs.Template
{
    public class ChecklistFieldValueDto
    {
        public int Id { get; set; }

        public int TemplateFieldId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string FieldType { get; set; } = string.Empty;

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public string? Value { get; set; }
    }
}
