namespace TaskManager.Domain.DTO
{
    //Modelo de entrada de dados recebidos externamente para a criação de um Assignment
    public class CreateAssignmentDTO
    {
        public string Title;
        public int User;
        public bool Completed;
    }
}
