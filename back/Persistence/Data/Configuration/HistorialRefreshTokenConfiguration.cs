using Domine.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations;
public class HistorialRefreshTokenConfiguration : IEntityTypeConfiguration<HistorialRefreshToken>
{

    public void Configure(EntityTypeBuilder<HistorialRefreshToken> builder)
    {
        builder.ToTable("HistorialRefreshToken");

        // Configura la columna IsActive como BIT(1) en MySQL
        builder.Property(e => e.IsActive)
            .HasColumnType("BIT(1)"); // Utiliza BIT(1) para un valor booleano (0 o 1)



        builder.Property(e => e.DateCreated).HasColumnType("datetime");
        builder.Property(e => e.DateExpired).HasColumnType("datetime");
        builder.Property(e => e.RefreshToken)
            .HasMaxLength(200)
            .IsUnicode(false);
        builder.Property(e => e.Token)
            .HasMaxLength(500)
            .IsUnicode(false);

        builder.HasOne(d => d.IdUserNavigation).WithMany(p => p.HistorialRefreshTokens)
            .HasForeignKey(d => d.IdUser)
            .HasConstraintName("FKHistorialUser");
    }

}