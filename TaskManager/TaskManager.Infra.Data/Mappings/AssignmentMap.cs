using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Models;

namespace TaskManager.Infra.Data.Mappings
{
    //Configurações do mapeamento de Assignments no banco, utilizado no TaskManagerContext
    public class AssignmentMap : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");

            builder.HasKey(x => x.AssignmentId);

            builder.Property(x => x.AssignmentId)
                .HasColumnType("integer")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Title)
                .HasColumnType("character")
                .HasColumnName("title");

            builder.Property(x => x.Completed)
                .HasColumnType("boolean")
                .HasColumnName("completed");

            builder.Property(x => x.UserId)
                .HasColumnType("integer")
                .HasColumnName("userId");

            //builder.HasOne(a => a.User)
            //    .WithMany(u => u.Assignments)
            //    .HasForeignKey(u => u.User.UserId);
        }
    }
}
