﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Model;

public partial class EmpPortalContext : DbContext
{
    public EmpPortalContext()
    {
    }

    public EmpPortalContext(DbContextOptions<EmpPortalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Approver> Approvers { get; set; }

    public virtual DbSet<ApproverH> ApproverHs { get; set; }

    public virtual DbSet<ApproverXemployee> ApproverXemployees { get; set; }

    public virtual DbSet<ApproverXemployeeH> ApproverXemployeeHs { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeH> EmployeeHs { get; set; }

    public virtual DbSet<EmployeeLogin> EmployeeLogins { get; set; }

    public virtual DbSet<EmployeeLoginH> EmployeeLoginHs { get; set; }

    public virtual DbSet<H1bentriesH> H1bentriesHes { get; set; }

    public virtual DbSet<H1bentry> H1bentries { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectXemployee> ProjectXemployees { get; set; }

    public virtual DbSet<ProjectXemployeeH> ProjectXemployeeHs { get; set; }

    public virtual DbSet<Superadmin> Superadmins { get; set; }

    public virtual DbSet<TimeSheet> TimeSheets { get; set; }

    public virtual DbSet<TimeSheetH> TimeSheetHs { get; set; }

    public virtual DbSet<VacationAppItem> VacationAppItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:envoyapp.database.windows.net,1433;Initial Catalog=EmployeePortal;Persist Security Info=False;User ID=zenvoadmin;Password=Kesh1v1@4321$%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Approver>(entity =>
        {
            entity.HasKey(e => e.ApproverId).HasName("PK__Approver__02547A7AA62C6212");

            entity.ToTable("Approver", tb => tb.HasTrigger("tr_Approver"));

            entity.Property(e => e.ApproverId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<ApproverH>(entity =>
        {
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<ApproverXemployee>(entity =>
        {
            entity.HasKey(e => e.Axeid).HasName("PK_ApproverXemployee");

            entity.ToTable("ApproverXEmployee", tb => tb.HasTrigger("tr_ApproverXEmployee"));

            entity.Property(e => e.Axeid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<ApproverXemployeeH>(entity =>
        {
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11DB4C0030");

            entity.ToTable("Employee", tb => tb.HasTrigger("tr_Employee"));

            entity.Property(e => e.EmployeeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Tsfreq).IsFixedLength();
            entity.Property(e => e.UpdatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WhatOperation).IsFixedLength();

            entity.HasOne(d => d.CreatedUserNavigation).WithMany(p => p.InverseCreatedUserNavigation).HasConstraintName("FK_Employee_CreatedUser");

            entity.HasOne(d => d.UpdatedUserNavigation).WithMany(p => p.InverseUpdatedUserNavigation).HasConstraintName("FK_Employee_UpdatedUser");
        });

        modelBuilder.Entity<EmployeeH>(entity =>
        {
            entity.Property(e => e.Tsfreq).IsFixedLength();
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<EmployeeLogin>(entity =>
        {
            entity.ToTable("EmployeeLogin", tb => tb.HasTrigger("tr_EmployeeLogin"));

            entity.Property(e => e.EmployeeId).ValueGeneratedNever();
            entity.Property(e => e.WhatOperation).IsFixedLength();

            entity.HasOne(d => d.Employee).WithOne(p => p.EmployeeLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeLogin_Employee");
        });

        modelBuilder.Entity<H1bentriesH>(entity =>
        {
            entity.Property(e => e.ApprovalStatus).HasDefaultValue(false);
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<H1bentry>(entity =>
        {
            entity.ToTable("H1Bentries", tb => tb.HasTrigger("tr_H1Bentries"));

            entity.Property(e => e.ApprovalStatus).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WhatOperation).IsFixedLength();

            entity.HasOne(d => d.CreatedUserNavigation).WithMany(p => p.ProjectCreatedUserNavigations).HasConstraintName("FK_Project_CreatedUser");

            entity.HasOne(d => d.UpdatedUserNavigation).WithMany(p => p.ProjectUpdatedUserNavigations).HasConstraintName("FK_Project_UpdatedUser");
        });

        modelBuilder.Entity<ProjectXemployee>(entity =>
        {
            entity.ToTable("ProjectXEmployee", tb => tb.HasTrigger("tr_ProjectXEmployee"));

            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WhatOperation).IsFixedLength();

            entity.HasOne(d => d.CreatedUserNavigation).WithMany().HasConstraintName("FK_ProjectXEmployee_CreatedUser");

            entity.HasOne(d => d.Employee).WithMany().HasConstraintName("FK_ProjectXEmployee_EmployeeId");

            entity.HasOne(d => d.Project).WithMany().HasConstraintName("FK_ProjectXEmployee_Project");

            entity.HasOne(d => d.UpdatedUserNavigation).WithMany().HasConstraintName("FK_ProjectXEmployee_UpdatedUser");
        });

        modelBuilder.Entity<ProjectXemployeeH>(entity =>
        {
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<Superadmin>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK_SuperAdmins");

            entity.Property(e => e.EmployeeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TimeSheet>(entity =>
        {
            entity.ToTable("TimeSheet", tb =>
                {
                    tb.HasTrigger("enforce_approver_mapping");
                    tb.HasTrigger("tr_TimeSheet");
                });

            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WhatOperation).IsFixedLength();

            entity.HasOne(d => d.CreatedUserNavigation).WithMany(p => p.TimeSheetCreatedUserNavigations).HasConstraintName("FK_TimeSheet_CreatedUser");

            entity.HasOne(d => d.Employee).WithMany(p => p.TimeSheetEmployees).HasConstraintName("FK_TimeSheet_EmployeeId");

            entity.HasOne(d => d.UpdatedUserNavigation).WithMany(p => p.TimeSheetUpdatedUserNavigations).HasConstraintName("FK_TimeSheet_UpdatedUser");
        });

        modelBuilder.Entity<TimeSheetH>(entity =>
        {
            entity.Property(e => e.WhatOperation).IsFixedLength();
        });

        modelBuilder.Entity<VacationAppItem>(entity =>
        {
            entity.Property(e => e.ApprovalStatus).HasDefaultValue("Pending");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
