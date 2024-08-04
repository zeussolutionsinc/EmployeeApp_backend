using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Table("superadmin")]
public partial class Superadmin
{
    [Key]
    [Column("employeeId")]
    public Guid EmployeeId { get; set; }
}
