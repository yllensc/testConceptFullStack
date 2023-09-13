using Domine.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations;
    public class ClientConfiguration: IEntityTypeConfiguration<Client>
    {

    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Client");
        builder.Property(r => r.IdentificationNumber)
        .IsRequired()
        .HasMaxLength(150);
        builder.HasIndex(r => r.IdentificationNumber)
        .IsUnique();
        builder.Property(r => r.Name)
        .IsRequired()
        .HasMaxLength(150);
        builder.Property(r => r.Email)
        .IsRequired()
        .HasMaxLength(150);
        builder.Property(r => r.PhoneNumber)
        .IsRequired();
    }
}