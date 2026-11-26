namespace Inspections.Models.DTOs.Template;

public class TemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int VersionCount { get; set; }
}

public class TemplateDetailDto : TemplateDto
{
    public List<TemplateVersionDto> Versions { get; set; } = new();
}

public class TemplateVersionDto
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public int VersionNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<TemplateFieldDto> Fields { get; set; } = new();
}

public class TemplateFieldDto
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public int TemplateVersionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? Options { get; set; }
}

public class CreateTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateTemplateFieldDto> Fields { get; set; } = new();
}

public class CreateTemplateFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? Options { get; set; }
}

public class UpdateTemplateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
