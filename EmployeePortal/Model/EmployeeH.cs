using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[PrimaryKey("EmployeeId", "StartDate")]
[Table("Employee_h", Schema = "history")]
public partial class EmployeeH
{
    [Key]
    public Guid EmployeeId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? LastName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [StringLength(225)]
    [Unicode(false)]
    public string? HomeAddress { get; set; }

    [Column("createdUser")]
    public Guid? CreatedUser { get; set; }

    [Column("updatedUser")]
    public Guid? UpdatedUser { get; set; }

    [Column("createdTime")]
    public DateTime? CreatedTime { get; set; }

    [Column("updatedTime")]
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

    [StringLength(5)]
    [Unicode(false)]
    public string? TimeSheet { get; set; }
}
