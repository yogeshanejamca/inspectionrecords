using Inspections.Models.Entities.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Inspections.Models.Entities.Templates
{
    public class TemplateField
    {

        public int Id { get; set; }

        public int TemplateId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string FieldType { get; set; } = string.Empty;

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public string? Options { get; set; }

        public int TemplateVersionId { get; set; }

        public TemplateVersion TemplateVersion { get; set; } = null!;

    }
}
