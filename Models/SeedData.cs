namespace MyToDo.Models;

public class SeedData
{

	public static void Initialize(MyToDoContext context)
	{
        if (!context.Users.Any())
        {
            context.Users.Add(
                new User()
                {
                    Username = "ssar",
                    Password = "1234",
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                }
            );
            context.SaveChanges();

        } 

        if (!context.ToDos.Any())
        {
            context.ToDos.Add(
                new ToDo()
                {
                    UserId = 1,
                    Content = "sample",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                }
            );
            context.SaveChanges();
        }

        
    }
}


