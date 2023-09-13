using Domine.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Persistence.Data.Configurations;
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {

    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Rol");
        builder.Property(r => r.RolName)
        .IsRequired()
        .HasMaxLength(150);
        builder.HasIndex(r => r.RolName)
        .IsUnique();
    }
}