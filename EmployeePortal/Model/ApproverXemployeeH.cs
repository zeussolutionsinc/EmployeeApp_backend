using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[PrimaryKey("Id", "StartDate")]
[Table("ApproverXEmployee_h", Schema = "history")]
public partial class ApproverXemployeeH
{
    [Key]
    public short Id { get; set; }

    public Guid? Approver { get; set; }

    [Column("EmployeeID")]
    public Guid? EmployeeId { get; set; }

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
}
