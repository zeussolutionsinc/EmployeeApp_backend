using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("ApproverXEmployee")]
public partial class ApproverXemployee
{
    public Guid? Approver { get; set; }

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

    [Key]
    [Column("AXEId")]
    public Guid Axeid { get; set; }
}
