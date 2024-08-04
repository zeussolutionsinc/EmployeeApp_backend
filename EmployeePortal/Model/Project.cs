using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("Project")]
public partial class Project
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string ProjectId { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? ProjectName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ClientName { get; set; }

    [Column("what_operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? WhatOperation { get; set; }

    [Column("created_user")]
    public Guid? CreatedUser { get; set; }

    [Column("created_time")]
    public DateTime? CreatedTime { get; set; }

    [Column("updated_user")]
    public Guid? UpdatedUser { get; set; }

    [Column("updated_time")]
    public DateTime? UpdatedTime { get; set; }

    [ForeignKey("CreatedUser")]
    [InverseProperty("ProjectCreatedUserNavigations")]
    public virtual Employee? CreatedUserNavigation { get; set; }

    [ForeignKey("UpdatedUser")]
    [InverseProperty("ProjectUpdatedUserNavigations")]
    public virtual Employee? UpdatedUserNavigation { get; set; }
}
