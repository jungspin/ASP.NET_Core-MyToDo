using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace MyToDo.Models
{
    /// <summary>
    /// 작성된 ToDo
    /// </summary>
    public class ToDo 
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
		public string Content { get; set; }
        // see: https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-mvc-app/new-field?view=aspnetcore-8.0&tabs=visual-studio
        [Display(Name = "완료여부")]
        [Column("is_done")]
        public int IsDone { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }

		public virtual User User { get; set; }
	}
}

