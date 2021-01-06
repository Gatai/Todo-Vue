using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace todo_Api.Models
{
    public class Todo
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}
