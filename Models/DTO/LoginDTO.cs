using System;
namespace MyToDo.Models.DTO
{
	public class LoginDTO
	{
		public string Username { get; set; }
		// 암호화 필요
		//public string Password { get => Password; set => this.Password = value; }
		public string Password { get; set; }
		//public LoginDTO(string username, string password)
		//{
		//	this.Username = username;
		//	this.Password = password;
		//}
	}
}

