using Microsoft.EntityFrameworkCore;

namespace DotNetLib.EfCore.EntityAudit
{
    public static class DbContextExtensions
    {
        public static void EnsureEntityAudit(this DbContext context, string identityId)
        {
            var modifiedEntries = context.ChangeTracker.Entries()
                            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedEntries)
            {
                if (entry.State == EntityState.Modified)
                {
                    SetProperty(entry, "ChangedOn", DateTime.Now);
                    SetProperty(entry, "ChangedBy", identityId);
                }

                if (entry.State == EntityState.Added)
                {
                    SetProperty(entry, "CreatedOn", DateTime.Now);
                    SetProperty(entry, "CreatedBy", identityId);
                }
            }
        }

        private static void SetProperty(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string propertyName, object value)
        {
            var entityType = entry.Context.Model.FindEntityType(entry.Entity.GetType());
            var prop = entityType.FindProperty(propertyName);
            if (prop != null)
            {
                entry.Property(propertyName).CurrentValue = value;
            }
        }
    }
}
