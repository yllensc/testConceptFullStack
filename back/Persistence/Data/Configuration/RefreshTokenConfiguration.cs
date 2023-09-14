using Domine.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations;
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenOp2>
{

    public void Configure(EntityTypeBuilder<RefreshTokenOp2> builder)
    {
        builder.ToTable("RefreshToken");
    }
}