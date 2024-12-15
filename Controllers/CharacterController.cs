using DotaRPG.Data;
using DotaRPG.DbModel;
using DotaRPG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static DotaRPG.Controllers.CharacterController;

namespace DotaRPG.Controllers
{
	public class CharacterController : Controller
	{
		private readonly AppDbContext _context;
		private readonly ILogger _logger;
		public CharacterController(AppDbContext context, ILogger<Character> logger)
		{
			_context = context;
			_logger = logger;
		}

		public IActionResult ViewAllCharacters(string searchName)
		{
			var query = _context.Characters
					.Include(c => c.Class)
					.AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchName))
			{
				query = query.Where(c => c.Name.Contains(searchName));
			}

			AllCharactersViewModel model = new AllCharactersViewModel
			{
				Characters = query.ToList(),
				CharacterClasses = _context.CharacterClasses
					.ToDictionary(c => c.Id, c => c.Name)
			};

			ViewData["SearchName"] = searchName;

			return View(model);
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			var character = _context.Characters.Find(id);
			if (character != null)
			{
				_context.Characters.Remove(character);
				_context.SaveChanges();
			}
			return Json(new { success = true });
		}

		// Т.к ModelState.IsValid просит поле Class, я создал DTO
		public class CharacterDTO
		{
			public string Name { get; set; }
			public int Level { get; set; }
			public int Experience { get; set; }
			public int ClassId { get; set; }
		}


		[HttpPost]
		public IActionResult Create([FromBody] CharacterDTO characterDto)
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

			var character = new Character
			{
				Name = characterDto.Name,
				Level = characterDto.Level,
				Experience = characterDto.Experience,
				ClassId = characterDto.ClassId
			};

			try
			{
				_context.Characters.Add(character);
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
				id = character.Id
			});
		}
	}
}
