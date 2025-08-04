using TaskManager.Domain.Models;

namespace TaskManager.Application.ViewModels
{
    //Representação dos dados para serem utilizados na camada de apresentação
    public class AssignmentViewModel
    {

        public AssignmentViewModel(Assignment assignment)
        {
            Id = assignment.AssignmentId;
            Title = assignment.Title;
            User = assignment.UserId;
            Completed = assignment.Completed;
        }

        public AssignmentViewModel()
        {
            
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int User { get; set; }
        public bool Completed { get; set; }
    }
}
