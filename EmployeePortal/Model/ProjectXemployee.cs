using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Keyless]
[Table("ProjectXEmployee")]
public partial class ProjectXemployee
{
    [StringLength(10)]
    [Unicode(false)]
    public string? ProjectId { get; set; }

    [Column("EmployeeID")]
    public Guid? EmployeeId { get; set; }

    public Guid? CreatedUser { get; set; }

    public Guid? UpdatedUser { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? WhatOperation { get; set; }

    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("CreatedUser")]
    public virtual Employee? CreatedUserNavigation { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    [ForeignKey("UpdatedUser")]
    public virtual Employee? UpdatedUserNavigation { get; set; }
}
