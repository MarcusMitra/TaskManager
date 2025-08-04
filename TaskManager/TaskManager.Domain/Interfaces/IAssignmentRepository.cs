using TaskManager.Domain.DTO;
using TaskManager.Domain.Models;

namespace TaskManager.Domain.Interfaces
{
    //O que pode ser feito para acessar a DB em relação à classe Assignment
    public interface IAssignmentRepository
    {
        IQueryable<Assignment> GetAllAssignments();

        Assignment GetById(int id);

        Assignment UpdateAssignment(Assignment assignment);

        void DeleteAssignment(int id);

        Assignment CreateAssignment(CreateAssignmentDTO assignment);

        void AddRange(List<Assignment> assignments);

        Task<Assignment?> GetByIdAsync(int id);

        Task UpdateAsync(Assignment assignment);

        Task<int> CountIncompleteAssignmentsByUserIdAsync(int userId);
    }
}
