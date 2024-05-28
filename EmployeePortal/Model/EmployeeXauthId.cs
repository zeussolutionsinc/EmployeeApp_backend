using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

[Keyless]
[Table("EmployeeXAuthID")]
public partial class EmployeeXauthId
{
    [Column("EmployeeID")]
    public Guid? EmployeeId { get; set; }

    [Column("AuthID")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AuthId { get; set; }
}
