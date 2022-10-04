﻿using Microsoft.EntityFrameworkCore;
using QRA.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRA.Persistence.Setup
{
    public static  class TenantSetup
    {

        public static void ConfigureTenant (this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.IdTenant)
                    .HasName("PK__tenants__77F91E47AE3DE5A4");

                entity.ToTable("tenants", "qra");

                entity.Property(e => e.IdTenant).HasColumnName("idTenant");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("fullname");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone");
            });

        }

    }
}
