using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Models;
using TaskManager.Infra.Data.Mappings;

namespace TaskManager.Infra.Data.Context
{
    //DbContext do Entity Framework, define a criação das tabelas
    public sealed class TaskManagerContext : DbContext
    {
        public TaskManagerContext (DbContextOptions<TaskManagerContext> options ) : base( options ) { }

        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AssignmentMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
