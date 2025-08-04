using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.DTO;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Models;
using TaskManager.Infra.Data.Context;

namespace TaskManager.Infra.Data.Repository
{
    //Implementação dos métodos de IAssignmentRepository, acessa o banco de dados
    public class AssignmentRepository : IAssignmentRepository
    {
        protected readonly TaskManagerContext _db;
        protected readonly DbSet<Assignment> _dbSet;

        public AssignmentRepository(TaskManagerContext context)
        {
            _db = context;
            _dbSet = _db.Set<Assignment>();
        }

        public void AddRange(List<Assignment> assignments)
        {
            _dbSet.AddRange(assignments);
            _db.SaveChanges();
        }

        public Assignment CreateAssignment(CreateAssignmentDTO dto)
        {
            var assignment = new Assignment
            {
                Title = dto.Title,
                Completed = dto.Completed,
                UserId = dto.User
            };

            _dbSet.Add(assignment);
            _db.SaveChanges();

            return assignment;
        }

        public void DeleteAssignment(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Assignment> GetAllAssignments()
        {
            return _db.Assignments.AsQueryable();
        }

        public Assignment GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Assignment UpdateAssignment(Assignment assignment)
        {
            throw new NotImplementedException();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _db.Assignments.FindAsync(id);
        }

        public async Task UpdateAsync(Assignment assignment)
        {
            _db.Assignments.Update(assignment);
            await _db.SaveChangesAsync();
        }

        public async Task<int> CountIncompleteAssignmentsByUserIdAsync(int userId)
        {
            return await _db.Assignments
                .CountAsync(a => a.UserId == userId && !a.Completed);
        }
    }
}
