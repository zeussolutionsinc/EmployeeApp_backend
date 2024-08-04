using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

public partial class VacationAppItem
{
    [Key]
    public long Id { get; set; }

    public string? Name { get; set; }

    public bool IsComplete { get; set; }

    public string? Body { get; set; }

    public string? Secret { get; set; }

    public DateOnly VacationStartdate { get; set; }

    public DateOnly VacationEnddate { get; set; }

    [Column("isManager")]
    public bool IsManager { get; set; }

    public string? ImageUrl { get; set; }

    public string? Endhours { get; set; }

    public string? Starthours { get; set; }

    [Column("agree")]
    public bool Agree { get; set; }

    [Column("fileupload")]
    public string? Fileupload { get; set; }

    public string? Email { get; set; }

    public string? AuthId { get; set; }

    [Column("approval_status")]
    public string ApprovalStatus { get; set; } = null!;
}
