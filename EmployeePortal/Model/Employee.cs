using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("Employee")]
public partial class Employee
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

    [StringLength(255)]
    [Unicode(false)]
    public string? HomeAddress { get; set; }

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

    [Column("TSFreq")]
    [StringLength(5)]
    [Unicode(false)]
    public string? Tsfreq { get; set; }

    [InverseProperty("CreatedUserNavigation")]
    public virtual ICollection<Approver> ApproverCreatedUserNavigations { get; set; } = new List<Approver>();

    [InverseProperty("Employee")]
    public virtual ICollection<Approver> ApproverEmployees { get; set; } = new List<Approver>();

    [InverseProperty("UpdatedUserNavigation")]
    public virtual ICollection<Approver> ApproverUpdatedUserNavigations { get; set; } = new List<Approver>();

    [ForeignKey("CreatedUser")]
    [InverseProperty("InverseCreatedUserNavigation")]
    public virtual Employee? CreatedUserNavigation { get; set; }

    [InverseProperty("Employee")]
    public virtual EmployeeLogin? EmployeeLogin { get; set; }

    [InverseProperty("CreatedUserNavigation")]
    public virtual ICollection<Employee> InverseCreatedUserNavigation { get; set; } = new List<Employee>();

    [InverseProperty("UpdatedUserNavigation")]
    public virtual ICollection<Employee> InverseUpdatedUserNavigation { get; set; } = new List<Employee>();

    [InverseProperty("CreatedUserNavigation")]
    public virtual ICollection<Project> ProjectCreatedUserNavigations { get; set; } = new List<Project>();

    [InverseProperty("UpdatedUserNavigation")]
    public virtual ICollection<Project> ProjectUpdatedUserNavigations { get; set; } = new List<Project>();

    [InverseProperty("CreatedUserNavigation")]
    public virtual ICollection<TimeSheet> TimeSheetCreatedUserNavigations { get; set; } = new List<TimeSheet>();

    [InverseProperty("Employee")]
    public virtual ICollection<TimeSheet> TimeSheetEmployees { get; set; } = new List<TimeSheet>();

    [InverseProperty("UpdatedUserNavigation")]
    public virtual ICollection<TimeSheet> TimeSheetUpdatedUserNavigations { get; set; } = new List<TimeSheet>();

    [ForeignKey("UpdatedUser")]
    [InverseProperty("InverseUpdatedUserNavigation")]
    public virtual Employee? UpdatedUserNavigation { get; set; }
}
