using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyToDo.Controllers.Filter;
using MyToDo.Models;
using MyToDo.Models.DTO;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MyToDo.Controllers
{
    public class MemberController : Controller
    {
        private readonly MyToDoContext _context;

        private readonly ILogger<HomeController> _logger;

        public MemberController(MyToDoContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "값을 입력해주세요.";
                return View(loginDTO);
            }
            if (_context.Users == null)
            {
                return BadRequest();
            }
            // SELECT * FROM USER WHERE USERNAME = ':loginDTO.Username' AND PASSWORD = ':loginDTO.Password' ;
            var result = (from user in _context.Users where (user.Username == loginDTO.Username && user.Password == loginDTO.Password) select user).ToList();
         
            if (!result.Any()) // 값이 비어있는지 확인 (== isNotEmpty) 일치하는 것이 없으면
            {
                ViewBag.Message = "아이디 또는 비밀번호가 일치하지 않습니다.";
                return View(loginDTO);
            }
            else // 일치하는 것이 있으면
            {
                // 쿠키 발급
                var user = result[0];
                HttpContext.Session.SetString(CheckSession.MySession, JsonConvert.SerializeObject(new LoginUserDTO(user.Id, user.Username)));
                ClaimsIdentity identity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, "User")}, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme, principal: principal);

                // 임시 데이터
                TempData["LoginUser"] = JsonConvert.SerializeObject(result[0]);

                // return Content("Login Success"); // View를 반환하지 않고 텍스트를 반환
                // 다른 컨트롤러의 액션으로 리다이렉션하면서 데이터 전달
                // return RedirectToAction("OtherAction", "OtherController", new { id = 123 });
                // see : https://learn.microsoft.com/ko-kr/aspnet/mvc/overview/older-versions-1/controllers-and-routing/aspnet-mvc-controllers-overview-cs
                return RedirectToAction(actionName: "Index", controllerName: "ToDo", routeValues: ToDoShowType.ToDo);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // GET: Member
        public async Task<IActionResult> Members()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'MyToDoContext.Users'  is null.");
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Member/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Member/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            //user.Created = DateTime.Now;
            //user.Updated = DateTime.Now;
            //return Content($"{user.ToString()}");

            user.Created = DateTime.Now;
            user.Updated = DateTime.Now;

            // ModelState.IsValid 왜 false인거지?
            if (user.Username.Equals("") && user.Password.Equals(""))
            {
                ViewBag.Message = "회원가입에 실패했습니다. 다시 시도 해주세요.";
                return View(user);
            }

            _context.Add(user);
            await _context.SaveChangesAsync();
            // 알림을 주어야만..
            return RedirectToAction(nameof(Login));
        }

        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Member/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Password,Created,Updated")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Members));
            }
            return View(user);
        }

        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'MyToDoContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Members));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
