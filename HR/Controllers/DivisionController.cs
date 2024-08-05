using HR.Context;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR.Controllers
{
    public class DivisionController : Controller
    {
        private readonly HrContext _context;

        public DivisionController(HrContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<StructuralDivision> structuralDivisions = await _context.StructuralDivisions.AsNoTracking()
                .Include(p => p.PositionChief)
                .ThenInclude(e => e.Employee)
                .ThenInclude(p => p.Person)
                .Where(sd => sd.DateCreation.Date <= DateTime.Now.Date && sd.DateLiquidation.Date >= DateTime.Now.Date)
                .ToListAsync();
            return View(structuralDivisions);
        }

        public async Task<IActionResult> CreateOrEditStructuralDivision(int id)
        {
            List<Position> positions = await _context.Positions
                .Where(p => p.DateCreationPosition.Date <= DateTime.Now.Date 
                && p.DateLiquidationPosition.Date >= DateTime.Now.Date)
                .ToListAsync();
            ViewBag.Positions = new SelectList(positions, "Id", "NamePosition");
            List<StructuralDivision> divisions = await _context.StructuralDivisions.Where(d => d.Id != id)
                .ToListAsync();
            ViewBag.Divisions = new SelectList(divisions, "Id", "FullTitle");
            StructuralDivision structuralDivision = new StructuralDivision();
            structuralDivision.DateCreation = DateTime.Now.Date;
            if (id != 0)
            {
                structuralDivision = await _context.StructuralDivisions.Where(sd => sd.Id == id)
                .FirstOrDefaultAsync();
            }
            return View("CreateOrEditStructuralDivision", structuralDivision);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrEditStructuralDivision(StructuralDivision structuralDivision)
        {
            try
            {
                int level = await _context.StructuralDivisions
                    .Where(s => s.Id == structuralDivision.ParentId)
                    .Select(l => l.Level).FirstOrDefaultAsync() + 1;
                structuralDivision.Level = level;
                if (structuralDivision?.Id != 0)
                {
                    StructuralDivision findSDInDb = await _context.StructuralDivisions.AsNoTracking()
                        .FirstOrDefaultAsync(sd => sd.Id == structuralDivision.Id);
                    if (findSDInDb != null)
                    {
                        structuralDivision.ModifiedDateRecord = DateTime.Now;
                       _context.StructuralDivisions.Update(structuralDivision);
                    }
                    else
                    {
                        return RedirectToAction("~/Views/Shared/Error.cshtml");
                    }
                }
                else
                {
                    structuralDivision.DateCreationRecord = DateTime.Now;
                    await _context.StructuralDivisions.AddAsync(structuralDivision);
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("~/Views/Shared/Error.cshtml");
            }           
        }
    }
}
