namespace MyToDo.Models.DTO
{
    public class LoginUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public LoginUserDTO(int userId, string username) { 
            Id = userId;
            Name = username;
        }
    }
}
