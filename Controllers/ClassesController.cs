using DotaRPG.Data;
using DotaRPG.DbModel;
using DotaRPG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DotaRPG.Controllers
{
	public class ClassesController : Controller
	{
		private readonly AppDbContext _context;
		private readonly ILogger _logger;
		public ClassesController(AppDbContext context, ILogger<ClassesController> logger)
		{
			_context = context;
			_logger = logger;
		}

		public IActionResult ViewAllClasses(string searchName)
		{
			var query = _context.CharacterClasses.AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchName))
			{
				query = query.Where(c => c.Name.Contains(searchName));
			}

			var model = new AllClassesViewModel
			{
				CharacterClasses = query.ToList()
			};

			ViewData["SearchName"] = searchName;

			return View(model);
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			var characterClass = _context.CharacterClasses.Find(id);
			if (characterClass != null)
			{
				_context.CharacterClasses.Remove(characterClass);
				_context.SaveChanges();
			}
			return Json(new { success = true });
		}


		// Вообще можно было по сути дженериками сделать все, чтобы 2 раза код не дублировать
		[HttpPost]
		public IActionResult Create([FromBody] CharacterClass characterClass)
		{
			if (!ModelState.IsValid)
			{
				return Json(new
				{
					success = false,
					errors = ModelState.Values
						.SelectMany(e => e.Errors)
						.Select(e => e.ErrorMessage)
						.ToList()
				});
			}

			// На всякий случай, вдруг null передадут))
			try
			{
				_context.CharacterClasses.Add(characterClass);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при создании класса персонажа");

				return Json(new
				{
					success = false,
					error = "Произошла ошибка при сохранении данных"
				});
			}

			return Json(new
			{
				success = true,
				id = characterClass.Id
			});
		}
	}
}
