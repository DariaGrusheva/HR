using HR.Context;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace HR.Controllers
{
    public class PersonController : Controller
    {

        private readonly HrContext _context;
        IWebHostEnvironment _appEnvironment;

        public PersonController(HrContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            List<Person> persons = await _context.Persons.AsNoTracking()
                .ToListAsync();
            return View(persons);
        }

        public async Task<IActionResult> DetailsPerson(int id)
        {
            Person? person = await _context.Persons.AsNoTracking()
                .Include(p => p.Employees)
                .ThenInclude(p => p.Position)
                .ThenInclude(sd => sd.StructuralDivision)
                .FirstOrDefaultAsync(e => e.Id == id);
            person.Experience = await CalculationExperience(person.Id);
            return View("DetailsPerson", person);

        }

        public async Task<IActionResult> CreateOrEditPerson(int id)
        {
            List<string> gender = new List<string>() { "Муж", "Жен" };
            ViewBag.Genders = new SelectList(gender);
            Person person = new Person();
            if (id != 0)
            {
                person = await _context.Persons.Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            }
            return View("CreateOrEditPerson", person);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrEditPerson(Person person, IFormFile uploadPhoto)
        {
            try
            {
                if (uploadPhoto != null)
                {
                    using var fileStream = uploadPhoto.OpenReadStream();
                    byte[] bytes = new byte[uploadPhoto.Length];
                    fileStream.Read(bytes, 0, (int)uploadPhoto.Length);
                    person.Photo = bytes;
                }
                if (person?.Id != 0)
                {
                    Person findPersonInDb = await _context.Persons.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == person.Id);
                    if (findPersonInDb != null)
                    {
                        person.ModifiedDateRecord = DateTime.Now;
                        _context.Persons.Update(person);
                    }
                    else
                    {
                        return RedirectToAction("~/Views/Shared/Error.cshtml");
                    }
                }
                else
                {
                    person.DateCreationRecord = DateTime.Now;
                    await _context.Persons.AddAsync(person);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("~/Views/Shared/Error.cshtml");
            }

        }

        private async Task<int> CalculationExperience(int personId)
        {
            double experienceDays = 0;
            List<Employee> employees = await _context.Employees.Where(e => e.PersonId == personId)
                .OrderByDescending(e => e.DateDismissalTransfer)
                .ToListAsync();
            foreach (Employee em in employees)
            {
                if (em.DateDismissalTransfer < DateTime.Now)
                {
                    experienceDays += (em.DateDismissalTransfer - em.DateAdmissionTransfer).TotalDays;
                }
                else
                {
                    experienceDays += (DateTime.Now - em.DateAdmissionTransfer).TotalDays;
                }
            }
            int experience = (int)experienceDays / 365;
            return experience;
        }

    }
}
