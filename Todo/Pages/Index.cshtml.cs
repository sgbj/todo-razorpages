using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.Models;

namespace Todo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TodoDbContext _db;

        public IndexModel(TodoDbContext db)
        {
            _db = db;
        }

        public List<TodoItem> TodoItems { get; private set; }

        [FromQuery]
        public TodoFilter TodoFilter { get; set; }

        [BindProperty]
        public TodoItem TodoItem { get; set; }

        public async Task OnGetAsync()
        {
            TodoItems = await _db.TodoItems.ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await _db.TodoItems.AddAsync(TodoItem);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            _db.Attach(TodoItem).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var item = await _db.TodoItems.FindAsync(TodoItem.Id);

            if (item != null)
            {
                _db.TodoItems.Remove(item);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearCompletedAsync()
        {
            _db.TodoItems.RemoveRange(_db.TodoItems.Where(i => i.IsCompleted));
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMarkAllAsync()
        {
            var items = await _db.TodoItems.ToListAsync();
            var isCompleted = items.All(i => i.IsCompleted);
            items.ForEach(i => i.IsCompleted = !isCompleted);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }

    public enum TodoFilter
    {
        All,
        Active,
        Completed
    }
}
