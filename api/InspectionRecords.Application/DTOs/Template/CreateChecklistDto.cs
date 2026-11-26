using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspections.Models.DTOs.Template
{
    public class CreateChecklistDto
    {
        public int InspectionId { get; set; }

        public int TemplateVersionId { get; set; }
    }
}
