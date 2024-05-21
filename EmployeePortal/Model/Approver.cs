using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("Approver")]
public partial class Approver
{
    [Key]
    public Guid ApproverId { get; set; }

    public Guid? EmployeeId { get; set; }

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
    [InverseProperty("ApproverCreatedUserNavigations")]
    public virtual Employee? CreatedUserNavigation { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("ApproverEmployees")]
    public virtual Employee? Employee { get; set; }

    [InverseProperty("ApprovedByNavigation")]
    public virtual ICollection<TimeSheet> TimeSheets { get; set; } = new List<TimeSheet>();

    [ForeignKey("UpdatedUser")]
    [InverseProperty("ApproverUpdatedUserNavigations")]
    public virtual Employee? UpdatedUserNavigation { get; set; }
}
