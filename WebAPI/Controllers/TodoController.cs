using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Data;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Route("api/todos")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/todos
        [HttpGet]
        public async Task<ActionResult<PaginatedList<TodoItem>>> GetTodoItems(
            string? searchTerm,
            string? sort,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.TodoItems.AsNoTracking().AsQueryable();

            query = ApplySearch(query, searchTerm);
            query = ApplySort(query, sort);

            var paginatedList = await PaginatedList<TodoItem>.CreateAsync(query, page, pageSize);
            return paginatedList;
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // POST: api/todos
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // PUT: api/todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            TodoItem? toUpdate = await _context.TodoItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (toUpdate == null)
            {
                return NotFound();
            }

            toUpdate.Title = todoItem.Title;
            toUpdate.Description = todoItem.Description;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<TodoItem> ApplySearch(IQueryable<TodoItem> query, string? searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Title.Contains(searchTerm));
            }

            return query;
        }

        private IQueryable<TodoItem> ApplySort(IQueryable<TodoItem> query, string? sort)
        {
            if (string.IsNullOrEmpty(sort)) return query;

            query = sort.ToLower() switch
            {
                "title_asc" => query.OrderBy(t => t.Title),
                "title_desc" => query.OrderByDescending(t => t.Title),
                _ => query
            };

            return query;
        }
    }
}