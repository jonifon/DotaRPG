using DotaRPG.Data;
using DotaRPG.DbModel;
using DotaRPG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static DotaRPG.Controllers.CharacterController;

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

			//if (!string.IsNullOrWhiteSpace(searchName))
			//{
			//	query = query.Where(c => c.Name.Contains(searchName));
			//}
			//ViewData["SearchName"] = searchName;

			var model = new AllClassesViewModel
			{
				CharacterClasses = query.ToList()
			};

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


				AddChangeHistory("Remove", characterClass.Id, "", "", "");
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

				AddChangeHistory("Create", characterClass.Id, "", characterClass.Name, nameof(characterClass.Name));
				AddChangeHistory("Create", characterClass.Id, "", characterClass.Description, nameof(characterClass.Description));
				AddChangeHistory("Create", characterClass.Id, "", characterClass.BaseDamage.ToString(), nameof(characterClass.BaseDamage));
				AddChangeHistory("Create", characterClass.Id, "", characterClass.BaseHealth.ToString(), nameof(characterClass.BaseHealth));
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

		[HttpPost]
		public IActionResult Edit([FromBody] CharacterClass characterClass)
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

			var existingClass = _context.CharacterClasses.Find(characterClass.Id);
			if (existingClass == null)
			{
				return Json(new
				{
					success = false,
					error = "Класс не найден"
				});
			}

			if (existingClass.Name != characterClass.Name)
				AddChangeHistory("Update", existingClass.Id, existingClass.Name, characterClass.Name, nameof(characterClass.Name));

			if (existingClass.Description != characterClass.Description)
				AddChangeHistory("Update", existingClass.Id, existingClass.Description.ToString(), characterClass.Description.ToString(), nameof(characterClass.Description));

			if (existingClass.BaseDamage != characterClass.BaseDamage)
				AddChangeHistory("Update", existingClass.Id, existingClass.BaseDamage.ToString(), characterClass.BaseDamage.ToString(), nameof(characterClass.BaseDamage));

			if (existingClass.BaseHealth != characterClass.BaseHealth)
				AddChangeHistory("Update", existingClass.Id, existingClass.BaseHealth.ToString(), characterClass.BaseHealth.ToString(), nameof(characterClass.BaseHealth));


			existingClass.Name = characterClass.Name;
			existingClass.Description = characterClass.Description;
			existingClass.BaseDamage = characterClass.BaseDamage;
			existingClass.BaseHealth = characterClass.BaseHealth;

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
				EntityName = "Class",
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
