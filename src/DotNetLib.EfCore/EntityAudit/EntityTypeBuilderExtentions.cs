using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetLib.EfCore.EntityAudit
{
    public static class EntityTypeBuilderExtentions
    {
        public static void AddAuditingProperties(this EntityTypeBuilder builder)
        {
            builder.Property<DateTime?>("CreatedOn");
            builder.Property<string>("CreatedBy").HasMaxLength(50);
            builder.Property<DateTime?>("ChangedOn");
            builder.Property<string>("ChangedBy").HasMaxLength(50);
        }
    }
}
