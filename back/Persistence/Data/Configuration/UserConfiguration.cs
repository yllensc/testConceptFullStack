using Domine.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Persistence.Data.Configurations;
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.Property(u => u.UserName)
        .IsRequired()
        .HasColumnType("varchar")
        .HasMaxLength(150);
        builder.Property(u => u.Email)
        .IsRequired()
        .HasColumnType("varchar")
        .HasMaxLength(150);
        builder.Property(u => u.Password)
        .IsRequired()
        .HasColumnType("varchar")
        .HasMaxLength(250);

        builder
        .HasMany(u => u.Roles)
        .WithMany(u => u.Users)
        .UsingEntity<UserRol>(
            pk => pk
            .HasOne(u => u.Rol)
            .WithMany(u => u.UsersRoles)
            .HasForeignKey(u => u.IdRolFK),
            pk => pk
            .HasOne(u => u.User)
            .WithMany(u => u.UsersRoles)
            .HasForeignKey(u => u.IdUserFK),
            pk =>{
                pk.HasKey(ur => new {ur.IdUserFK, ur.IdRolFK});
            });
        
        builder.HasMany(p => p.RefreshTokens)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);


    }
}