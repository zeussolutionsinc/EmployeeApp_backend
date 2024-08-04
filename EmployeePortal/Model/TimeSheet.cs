using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("TimeSheet")]
public partial class TimeSheet
{
    [StringLength(10)]
    [Unicode(false)]
    public string? ProjectId { get; set; }

    [Column("EmployeeID")]
    public Guid? EmployeeId { get; set; }

    public DateOnly? WorkingDate { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? ApprovalStatus { get; set; }

    public Guid? ApprovedBy { get; set; }

    public Guid? CreatedUser { get; set; }

    public Guid? UpdatedUser { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? WhatOperation { get; set; }

    public short? WorkingHours { get; set; }

    [Key]
    public int RecordId { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? RecordNumber { get; set; }

    [ForeignKey("CreatedUser")]
    [InverseProperty("TimeSheetCreatedUserNavigations")]
    public virtual Employee? CreatedUserNavigation { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("TimeSheetEmployees")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("UpdatedUser")]
    [InverseProperty("TimeSheetUpdatedUserNavigations")]
    public virtual Employee? UpdatedUserNavigation { get; set; }
}
