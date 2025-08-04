using TaskManager.Application.ViewModels;

namespace TaskManager.Application.Interfaces
{
    //Funções que o AssignmentAppService fornece a API
    public interface IAssignmentAppService
    {
        List<AssignmentViewModel> GetAllAssignments(
            int page = 1,
            int pageSize = 10,
            string? title = null,
            string? sort = null,
            string? order = null);

        AssignmentViewModel? GetAssignment(int id);

        Task<bool> UpdateAssignmentStatusAsync(int id, bool completed);

        Task SyncAssignments ();
    }
}
