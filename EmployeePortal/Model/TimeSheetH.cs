using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[PrimaryKey("RecordId", "StartDate")]
[Table("TimeSheet_h", Schema = "history")]
public partial class TimeSheetH
{
    [Key]
    public int RecordId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? ProjectId { get; set; }

    [Column("EmployeeID")]
    public Guid? EmployeeId { get; set; }

    public DateOnly? WorkingDate { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? ApprovalStatus { get; set; }

    public Guid? CreatedUser { get; set; }

    public Guid? UpdatedUser { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }

    [Key]
    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("what_operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? WhatOperation { get; set; }

    public short? WorkingHours { get; set; }
}
