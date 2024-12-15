using DotaRPG.Data;
using DotaRPG.DbModel;
using DotaRPG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
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

		public IActionResult ViewAllCharacters(string searchName, string searchCharacterClass)
		{
			var query = _context.Characters
					.Include(c => c.Class)
					.AsQueryable();

			if (!string.IsNullOrEmpty(searchName))
			{
				query = query.Where(c => c.Name.Contains(searchName));
			}

			if (!string.IsNullOrEmpty(searchCharacterClass))
			{
				query = query.Where(c => c.Class.Name.Contains(searchCharacterClass));
			}

			AllCharactersViewModel model = new AllCharactersViewModel
			{
				Characters = query.ToList(),
				CharacterClasses = _context.CharacterClasses
					.ToDictionary(c => c.Id, c => c.Name)
			};

			ViewData["SearchName"] = searchName;
			ViewData["SearchCharacterClass"] = searchCharacterClass;

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

				AddChangeHistory("Remove", id, "", "", "");
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

				AddChangeHistory("Create", character.Id, "", character.Name, nameof(character.Name));
				AddChangeHistory("Create", character.Id, "", character.Level.ToString(), nameof(character.Level));
				AddChangeHistory("Create", character.Id, "", character.Experience.ToString(), nameof(character.Experience));
				AddChangeHistory("Create", character.Id, "", character.ClassId.ToString(), nameof(character.ClassId));
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

		public class EditCharacterDto
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public int Level { get; set; }
			public int Experience { get; set; }
			public int ClassId { get; set; }
		}

		[HttpPost]
		public IActionResult Edit([FromBody] EditCharacterDto editCharacterDto)
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

			var existingCharacter = _context.Characters.Find(editCharacterDto.Id);
			if (existingCharacter == null)
			{
				return Json(new
				{
					success = false,
					error = "Класс не найден"
				});
			}

			if (existingCharacter.Name != editCharacterDto.Name)
				AddChangeHistory("Update", existingCharacter.Id, existingCharacter.Name, editCharacterDto.Name, nameof(editCharacterDto.Name));

			if (existingCharacter.Level != editCharacterDto.Level)
				AddChangeHistory("Update", existingCharacter.Id, existingCharacter.Level.ToString(), editCharacterDto.Level.ToString(), nameof(editCharacterDto.Level));

			if (existingCharacter.Experience != editCharacterDto.Experience)
				AddChangeHistory("Update", existingCharacter.Id, existingCharacter.Experience.ToString(), editCharacterDto.Experience.ToString(), nameof(editCharacterDto.Experience));

			if (existingCharacter.ClassId != editCharacterDto.ClassId)
				AddChangeHistory("Update", existingCharacter.Id, existingCharacter.ClassId.ToString(), editCharacterDto.ClassId.ToString(), nameof(editCharacterDto.ClassId));

			existingCharacter.Name = editCharacterDto.Name;
			existingCharacter.Level = editCharacterDto.Level;
			existingCharacter.Experience = editCharacterDto.Experience;
			existingCharacter.ClassId = editCharacterDto.ClassId;

			try
			{
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при редактировании класса персонажа");
				return Json(new
				{
					success = false,
					error = "Произошла ошибка при сохранении изменений"
				});
			}

			return Json(new { success = true });
		}

		private void AddChangeHistory(string changeType, int entityId, string oldValue, string newValue, string propertyName)
		{
			var changeHistory = new ChangeHistory
			{
				ChangeType = changeType,
				EntityName = "Character",
				EntityId = entityId,
				OldValue = oldValue,
				NewValue = newValue,
				PropertyName = propertyName
			};

			_context.ChangeHistories.Add(changeHistory);
			_context.SaveChanges();
		}
	}
}
