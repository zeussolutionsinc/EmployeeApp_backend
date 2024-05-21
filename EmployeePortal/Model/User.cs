using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Keyless]
[Table("User")]
public partial class User
{
    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Password { get; set; }
}
