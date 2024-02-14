using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyToDo.Models;
using MyToDo.Models.DTO;
using Newtonsoft.Json;
using MyToDo.Controllers.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MyToDo.Controllers
{
    // 세션 필터 적용
    [CheckSession]
    public class ToDoController : Controller
    {
        private readonly MyToDoContext _context;

        public ToDoController(MyToDoContext context)
        {
            _context = context;
            
        }

        private LoginUserDTO? GetLoginUser()
        {
            // 세션에서 값 가져오기
            var sessionString = HttpContext.Session.GetString(CheckSession.MySession);
            if (sessionString != null)
            {
                return JsonConvert.DeserializeObject<LoginUserDTO>(sessionString);
            }
            else
            {
                return null;
            }
        }

        // GET: ToDo
        public async Task<IActionResult> Index(ToDoShowType type)
        {
            var loginUser = GetLoginUser();
            if (loginUser == null)
            {
                return BadRequest();
            }
            string headerTitle = $"{loginUser.Name}의 ";
            IQueryable<ToDo> toDos = (from toDo in _context.ToDos where (toDo.User.Id == loginUser.Id) select toDo);
            switch (type)
            {
                case ToDoShowType.Total:
                    headerTitle += "전체 목록";
                    toDos = from toDo in _context.ToDos where (toDo.User.Id == loginUser.Id) orderby toDo.Updated descending select toDo;
                    break;
                case ToDoShowType.Complete:
                    headerTitle += "완료 목록";
                    toDos = from toDo in _context.ToDos where (toDo.User.Id == loginUser.Id && toDo.IsDone == 1) orderby toDo.Updated descending select toDo;
                    break;
                case ToDoShowType.ToDo:
                    headerTitle += "할일 목록";
                    toDos = from toDo in _context.ToDos where (toDo.User.Id == loginUser.Id && toDo.IsDone == 0) orderby toDo.Updated descending select toDo;
                    break;
            }

            ViewBag.HeaderTitle = headerTitle;
            ViewBag.LoginUsername = loginUser.Name;
            return View(await toDos.ToListAsync());
        } // Index

        // GET: ToDo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // GET: ToDo/Create/
        public IActionResult Create()
        {
            var loginUser = GetLoginUser();
            if (loginUser == null)
            {
                return BadRequest();
            }

            ViewBag.LoginUsername = loginUser.Name;
            ViewBag.LoginUserId = loginUser.Id;
            return View();
        }

        // POST: ToDo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId, Content")] ToDoDTO toDoDTO)
        {
            if (!ModelState.IsValid)
            {
                //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", toDo.UserId);
                ViewBag.Message = "할일 저장에 실패했습니다. 다시 시도해주세요.";
                return View(toDoDTO);
            }

            ToDo toDo = new ToDo()
            {
                UserId = toDoDTO.UserId,
                Content = toDoDTO.Content,
                IsDone = 0,
                Created = DateTime.Now,
                Updated = DateTime.Now,
            }; 

            _context.Add(toDo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ToDo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", toDo.UserId);
            return View(toDo);
        }

        // POST: ToDo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Content,IsDone,Created,Updated")] ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", toDo.UserId);
            return View(toDo);
        }

        // GET: ToDo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // POST: ToDo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDos == null)
            {
                return Problem("Entity set 'MyToDoContext.ToDos'  is null.");
            }
            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo != null)
            {
                _context.ToDos.Remove(toDo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoExists(int id)
        {
          return (_context.ToDos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
