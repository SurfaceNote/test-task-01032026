using FinanceService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceService.Infrastructure.Persistence.Configurations;

public class UserFavoriteCurrenciesConfiguration : IEntityTypeConfiguration<UserFavoriteCurrency>
{
    public void Configure(EntityTypeBuilder<UserFavoriteCurrency> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CurrencyId)
            .IsRequired();
        
        builder.HasOne(e => e.Currency)
            .WithMany(c => c.FavoriteByUsers)
            .HasForeignKey(e => e.CurrencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}