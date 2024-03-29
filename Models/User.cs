﻿using System.ComponentModel.DataAnnotations.Schema;

namespace MyToDo.Models
{

    /// <summary>
    /// 투두 작성자
    /// </summary>
    public class User
    {
        // 작성자 번호 (PK)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // 작성자 아이디
        public string Username { get; set; }
        // 작성자 비밀번호
        // 암호화 필요
        //public string Password { get => Password; set => this.Password = value; } //throw exception
        public string Password { get; set; }

        // 작성자 계정 생성일
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        // 일대다
        public virtual ICollection<ToDo> ToDos { get; set; }

        public override string ToString()
        {
            return @$"번호 : {Id} / 이름: {Username} / 비밀번호 : {Password} / 생성일 : {Created} / 수정일 : {Updated}";
        }

    }
}
