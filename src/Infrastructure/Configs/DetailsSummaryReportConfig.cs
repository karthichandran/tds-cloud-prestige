
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReProServices.Domain.Entities;


namespace ReProServices.Infrastructure.Configs
{
    public class DetailsSummaryReportConfig : IEntityTypeConfiguration<DetailsSummaryReport>
    {
        public void Configure
           (EntityTypeBuilder<DetailsSummaryReport> entity)
        {
            entity.ToTable("DetailsSummaryReport").HasNoKey();
        }
    }
}
