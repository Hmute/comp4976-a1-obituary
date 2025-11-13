
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1.Models;
using Assignment1.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


public class ObituaryController : Controller
{
    private const string BearerScheme = "Identity.Bearer";
    private readonly ApplicationDbContext _context;

    public ObituaryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // QUICKGRID CHANGE: Updated Index to return materialized list for MVC view compatibility  
    // GET: OBITUARYS
    public async Task<IActionResult> Index()
    {
        // Return materialized list for MVC view
        return View(await _context.Obituaries.ToListAsync());
    }


    // GET: api/obituary/all (JSON API endpoint)
    [HttpGet("api/obituary/all")]
    public async Task<ActionResult<object>> GetObituaries(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Limit max page size

        var totalCount = await _context.Obituaries.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var obituaries = await _context.Obituaries
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new
        {
            Data = obituaries,
            Pagination = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            }
        };

        return Ok(result);
    }


    // GET: OBITUARYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var obituary = await _context.Obituaries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (obituary == null)
        {
            return NotFound();
        }

        return View(obituary);
    }


    // GET: api/obituary/Details/5
    [HttpGet("api/obituary/details/{id}")]
    public async Task<ActionResult<Obituary>> GetObituaryDetails(int id)
    {
        var obituary = await _context.Obituaries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (obituary == null)
        {
            return NotFound();
        }

        return obituary;
    }





    // GET: OBITUARYS/Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: OBITUARYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Obituary obituary)
    {
        if (ModelState.IsValid)
        {
            // Set the creator's user ID
            obituary.CreatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            obituary.CreatedAtUtc = DateTime.UtcNow;

            _context.Add(obituary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(obituary);
    }


    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpPost("/api/obituary")]
    public async Task<ActionResult<Obituary>> CreateObituary([FromBody] Obituary obituary)
    {
        if (ModelState.IsValid)
        {
            // Set the creator's user ID
            obituary.CreatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            obituary.CreatedAtUtc = DateTime.UtcNow;

            _context.Add(obituary);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetObituaryDetails), new { id = obituary.Id }, obituary);
        }
        return BadRequest(ModelState);
    }




    // GET: OBITUARYS/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var obituary = await _context.Obituaries.FindAsync(id);
        if (obituary == null)
        {
            return NotFound();
        }

        // Check if user can modify this obituary
        if (!CanModifyObituary(obituary))
        {
            return Forbid();
        }

        return View(obituary);
    }


    // POST: OBITUARYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Obituary obituary)
    {
        if (id != obituary.Id)
        {
            return NotFound();
        }

        // Check authorization before processing
        if (!await CanModifyObituaryAsync(obituary.Id))
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Preserve original creator info
                var existingObituary = await _context.Obituaries.AsNoTracking().FirstAsync(o => o.Id == obituary.Id);
                obituary.CreatedByUserId = existingObituary.CreatedByUserId;
                obituary.CreatedAtUtc = existingObituary.CreatedAtUtc;

                _context.Update(obituary);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObituaryExists(obituary.Id))
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
        return View(obituary);
    }


    // PUT: api/obituary/{id}
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpPut("/api/obituary/{id}")]
    public async Task<IActionResult> UpdateObituary(int id, [FromBody] Obituary obituary)
    {
        // Check authorization
        if (!await CanModifyObituaryAsync(id))
        {
            return StatusCode(403);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Preserve original creator info
        var existingObituary = await _context.Obituaries.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        if (existingObituary == null)
        {
            return NotFound();
        }

        obituary.CreatedByUserId = existingObituary.CreatedByUserId;
        obituary.CreatedAtUtc = existingObituary.CreatedAtUtc;

        _context.Entry(obituary).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ObituaryExists(obituary.Id))
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


    // DELETE: api/obituary/{id}
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpDelete("/api/obituary/{id}")]
    public async Task<IActionResult> DeleteObituary(int id)
    {
        var obituary = await _context.Obituaries.FindAsync(id);
        if (obituary == null)
        {
            return NotFound();
        }

        // Check authorization
        if (!CanModifyObituary(obituary))
        {
            return StatusCode(403);
        }

        _context.Obituaries.Remove(obituary);
        await _context.SaveChangesAsync();

        return NoContent();
    }



    // GET: OBITUARYS/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var obituary = await _context.Obituaries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (obituary == null)
        {
            return NotFound();
        }

        // Check if user can modify this obituary
        if (!CanModifyObituary(obituary))
        {
            return Forbid();
        }

        return View(obituary);
    }

    // POST: OBITUARYS/Delete/5
    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var obituary = await _context.Obituaries.FindAsync(id);
        if (obituary == null)
        {
            return NotFound();
        }

        // Check authorization
        if (!CanModifyObituary(obituary))
        {
            return Forbid();
        }

        _context.Obituaries.Remove(obituary);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }



    [HttpGet]
    [Route("Obituary/Search")]
    public async Task<IActionResult> Search(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return RedirectToAction(nameof(Index));
        }

        var obituaries = await _context.Obituaries.Where(obit => obit.FullName.ToLower().Contains(name.ToLower())).ToListAsync();

        return View(obituaries);
    }

    // GET: api/obituary/search (API endpoint for Blazor)
    [HttpGet("api/obituary/search")]
    public async Task<ActionResult<List<Obituary>>> SearchObituaries(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return await _context.Obituaries.ToListAsync();
        }

        var obituaries = await _context.Obituaries
            .Where(obit => obit.FullName.ToLower().Contains(name.ToLower()))
            .ToListAsync();

        return Ok(obituaries);
    }



    private bool ObituaryExists(int? id)
    {
        return _context.Obituaries.Any(e => e.Id == id);
    }

    // AUTHORIZATION HELPER: Determines if current user can modify an obituary
    // Implements ownership-based permissions and admin role access
    private bool CanModifyObituary(Obituary obituary)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        // Allow modification if user is admin OR the creator of the obituary
        return isAdmin || obituary.CreatedByUserId == currentUserId;
    }

    // AUTHORIZATION HELPER: Async version for checking modification permissions by ID
    private async Task<bool> CanModifyObituaryAsync(int obituaryId)
    {
        var obituary = await _context.Obituaries.FindAsync(obituaryId);
        if (obituary == null) return false;

        return CanModifyObituary(obituary);
    }
}
