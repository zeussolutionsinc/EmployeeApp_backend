using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("EmployeeLogin")]
public partial class EmployeeLogin
{
    [Key]
    public Guid EmployeeId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? EmployeeEmail { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? EmployeePassword { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? WhatOperation { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string? AuthId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeLogin")]
    public virtual Employee Employee { get; set; } = null!;
}
