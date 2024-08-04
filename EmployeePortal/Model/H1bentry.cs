using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("H1Bentries")]
public partial class H1bentry
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string PassportNumber { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? LegalFirstName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? LegalLastName { get; set; }

    public DateOnly? PassportExpiryDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ContactNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? HighestCollegeDegree { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CollegeCity { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CollegeName { get; set; }

    public int? GraduationYear { get; set; }

    public bool? Opt { get; set; }

    [Column("WorkedInUSA")]
    public bool? WorkedInUsa { get; set; }

    public int? YearsOfExperience { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CurrentEmployer { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CurrentJobTitle { get; set; }

    [Unicode(false)]
    public string? TechnicalSkills { get; set; }

    [Unicode(false)]
    public string? OtherSkills { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ReferralSource { get; set; }

    [Column("LinkedInURL")]
    [StringLength(200)]
    [Unicode(false)]
    public string? LinkedInUrl { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }

    [Column("what_operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? WhatOperation { get; set; }

    [Column("RegistrationID")]
    [StringLength(5)]
    [Unicode(false)]
    public string? RegistrationId { get; set; }

    public string? Resume { get; set; }

    [StringLength(75)]
    [Unicode(false)]
    public string? CreatedUser { get; set; }

    [StringLength(75)]
    [Unicode(false)]
    public string? UpdatedUser { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? DegreeMajor { get; set; }

    [Column("status")]
    public bool? Status { get; set; }

    [Column("approval_status")]
    [StringLength(50)]
    public string? ApprovalStatus { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string? AuthId { get; set; }
}
