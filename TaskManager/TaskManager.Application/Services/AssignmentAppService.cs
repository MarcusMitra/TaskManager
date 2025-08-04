using Newtonsoft.Json;
using System.Net.Http;
using TaskManager.Application.Interfaces;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Services
{
    //Implementação dos Casos de Uso com os Assignments
    public class AssignmentAppService : IAssignmentAppService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentAppService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public List<AssignmentViewModel> GetAllAssignments(
            int page = 1,
            int pageSize = 10,
            string? title = null,
            string? sort = null,
            string? order = null)
        {
            var query = _assignmentRepository.GetAllAssignments();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(a => a.Title.Contains(title));

            query = (sort?.ToLower(), order?.ToLower()) switch
            {
                ("title", "desc") => query.OrderByDescending(a => a.Title),
                ("title", _) => query.OrderBy(a => a.Title),
                ("userid", "desc") => query.OrderByDescending(a => a.UserId),
                ("userid", _) => query.OrderBy(a => a.UserId),
                ("completed", "desc") => query.OrderByDescending(a => a.Completed),
                ("completed", _) => query.OrderBy(a => a.Completed),
                _ => query.OrderBy(a => a.AssignmentId)
            };

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return query.Select(a => new AssignmentViewModel
            {
                Id = a.AssignmentId,
                Title = a.Title,
                Completed = a.Completed,
                User = a.UserId
            }).ToList();
        }


        public AssignmentViewModel? GetAssignment(int id)
        {
            Assignment? assignment = _assignmentRepository.GetById(id);

            if (assignment is null)
            {
                return null;
            }

            AssignmentViewModel assignmentViewModel = new(assignment);
            return assignmentViewModel;
        }

        public async Task SyncAssignments()
        {
            using HttpClient httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");

            HttpResponseMessage response = await httpClient.GetAsync("todos");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Erro na requisição: {response.StatusCode}");
                return;
            }

            string jsonContent = await response.Content.ReadAsStringAsync();
            var todos = JsonConvert.DeserializeObject<List<AssignmentViewModel>>(jsonContent);

            if (todos is null || todos.Count == 0)
            {
                Console.WriteLine("Nenhum dado encontrado na resposta.");
                return;
            }

            var assignments = todos
                .Where(p => p != null)
                .Select(p => new Assignment
                {
                    AssignmentId = p.Id,
                    Title = p.Title,
                    Completed = p.Completed,
                    UserId = p.User
                })
                .ToList();

            _assignmentRepository.AddRange(assignments);
        }
        public async Task<bool> UpdateAssignmentStatusAsync(int id, bool completed)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);

            if (assignment == null)
                throw new Exception("Tarefa não encontrada.");

            if (!completed && assignment.Completed)
            {
                int incompletas = await _assignmentRepository.CountIncompleteAssignmentsByUserIdAsync(assignment.UserId);

                if (incompletas >= 5)
                {
                    throw new InvalidOperationException("O usuário já possui 5 tarefas incompletas. Não é possível adicionar mais.");
                }
            }


            assignment.Completed = completed;
            await _assignmentRepository.UpdateAsync(assignment);
            return true;
        }

    }
}
