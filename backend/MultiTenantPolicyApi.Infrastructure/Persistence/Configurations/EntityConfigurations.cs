using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantPolicyApi.Domain.Entities;

namespace MultiTenantPolicyApi.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
    }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(c => c.TenantId);
        builder.HasOne(c => c.Tenant).WithMany(t => t.Customers).HasForeignKey(c => c.TenantId);
    }
}

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PolicyNumber).HasMaxLength(50).IsRequired();
        builder.Property(p => p.PremiumAmount).HasPrecision(18, 2);
        builder.HasIndex(p => new { p.TenantId, p.ExpirationDate });
        builder.HasIndex(p => p.PolicyNumber);
        builder.HasOne(p => p.Tenant).WithMany(t => t.Policies).HasForeignKey(p => p.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Customer).WithMany(c => c.Policies).HasForeignKey(p => p.CustomerId);
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasOne(u => u.Tenant).WithMany(t => t.Users).HasForeignKey(u => u.TenantId);
    }
}
