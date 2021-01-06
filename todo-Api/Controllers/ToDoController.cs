using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using todo_Api.Models;
using todo_Api.Repositories;
using System.Web.Http.Cors;

namespace todo_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly TodoContext _todoContext;

        public ToDoController(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var listTodos = _todoContext.Todos.ToList();

            return Ok(listTodos);
        }

        [HttpPost]
        public IActionResult Create(Todo model)
        {
            model.Id = Guid.NewGuid();

            _todoContext.Todos.Add(model);

            _todoContext.SaveChanges();

            return Ok(model);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(Guid id)
        {
            var todo = _todoContext.Todos.Single(m => m.Id == id);

            _todoContext.Remove(todo);

            _todoContext.SaveChanges();

            return Ok(_todoContext.Todos);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            var listTodos = _todoContext.Todos.Where(m => m.Completed).ToList();

            _todoContext.RemoveRange(listTodos);

            _todoContext.SaveChanges();

            return Ok(_todoContext.Todos);
        }

        [HttpPatch]
        [Route("{id}")]
        public IActionResult SetToCompleted(Guid id, bool completed)
        {
            var todo = _todoContext.Todos.Single(m => m.Id == id);

            todo.Completed = completed;

            _todoContext.Update(todo);
        
            _todoContext.SaveChanges();

            return Ok(_todoContext.Todos);
        }

        [HttpPatch]
        [Route("{id}/title")]
        public IActionResult UpdateTitle(Guid id, string title)
        {
            var todo = _todoContext.Todos.Single(m => m.Id == id);

            todo.Title = title;

            _todoContext.Update(todo);

            _todoContext.SaveChanges();

            return Ok(_todoContext.Todos);
        }

    }
}