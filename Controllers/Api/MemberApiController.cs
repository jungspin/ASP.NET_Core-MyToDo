using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyToDo.Models;

namespace MyToDo.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class JoinController : ControllerBase
{
    private readonly MyToDoContext _context;

    public JoinController(MyToDoContext context)
    {
        _context = context;
    }

    // GET: api/Join
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
      if (_context.Users == null)
      {
          return NotFound();
      }
        return await _context.Users.ToListAsync();
    }

    // GET: api/Join/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
      if (_context.Users == null)
      {
          return NotFound();
      }
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // PUT: api/Join/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Join
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
      if (_context.Users == null)
      {
          return Problem("Entity set 'MyToDoContext.Users'  is null.");
      }
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.Id }, user);
    }

    // DELETE: api/Join/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (_context.Users == null)
        {
            return NotFound();
        }
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
