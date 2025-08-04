namespace TaskManager.Domain.Models
{
    //Estrutura de Assignments mantida no banco de dados(Entidade do domínio)
    public class Assignment
    {
        public  int AssignmentId { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
        public int UserId { get; set; }
    }
}
