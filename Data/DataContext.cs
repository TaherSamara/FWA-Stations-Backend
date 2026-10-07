using FWA_Stations.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FWA_Stations.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public virtual DbSet<Users> Users { get; set; }
    public virtual DbSet<Devices> Devices { get; set; }
    public virtual DbSet<SubDevices> SubDevices { get; set; }
    public virtual DbSet<Warehouse> Warehouse { get; set; }
    public virtual DbSet<Permissions> Permissions { get; set; }
    public virtual DbSet<UserPermissions> UserPermissions { get; set; }

    public virtual DbSet<ErrorLogs> ErrorLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPermissions>(entity =>
        {
            entity.HasKey(up => up.id);

            entity.HasOne(up => up.user)
                  .WithMany(u => u.user_permissions)
                  .HasForeignKey(up => up.user_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(up => up.permission)
                  .WithMany(p => p.user_permissions)
                  .HasForeignKey(up => up.permission_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(up => !up.user.is_delete);
        });

        modelBuilder.Entity<SubDevices>(entity =>
        {
            entity.HasKey(sp => sp.id);

            entity.HasOne(sp => sp.device)
                  .WithMany(d => d.sub_devices)
                  .HasForeignKey(sp => sp.device_id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(w => w.id);

            entity.HasOne(w => w.assigned_user)
                  .WithMany()
                  .HasForeignKey(w => w.assigned_user_id)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        ApplyCommonAuditRelationships(modelBuilder);
    }
    private void ApplyCommonAuditRelationships(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            var entity = modelBuilder.Entity(clrType);

            // Apply global query filter for soft delete
            if (entityType.FindProperty("is_delete") != null)
            {
                var parameter = Expression.Parameter(clrType, "e");
                var property = Expression.Call(
                    typeof(EF).GetMethod(nameof(EF.Property)).MakeGenericMethod(typeof(bool)),
                    parameter,
                    Expression.Constant("is_delete"));

                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                entity.HasQueryFilter(lambda);
            }

            // Handle User audit
            if (typeof(BaseEntity).IsAssignableFrom(clrType))
            {
                if (entityType.FindProperty("insert_user_id") != null)
                {
                    entity.HasOne(typeof(Users), "insert_user")
                          .WithMany()
                          .HasForeignKey("insert_user_id")
                          .OnDelete(DeleteBehavior.Restrict);
                }

                if (entityType.FindProperty("update_user_id") != null)
                {
                    entity.HasOne(typeof(Users), "update_user")
                          .WithMany()
                          .HasForeignKey("update_user_id")
                          .OnDelete(DeleteBehavior.Restrict);
                }

                if (entityType.FindProperty("delete_user_id") != null)
                {
                    entity.HasOne(typeof(Users), "delete_user")
                          .WithMany()
                          .HasForeignKey("delete_user_id")
                          .OnDelete(DeleteBehavior.Restrict);
                }
            }
        }
    }
}
