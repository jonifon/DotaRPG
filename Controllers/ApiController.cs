using DotaRPG.Data;
using DotaRPG.DbModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using static DotaRPG.Controllers.CharacterController;

namespace DotaRPG.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ApiController : Controller
	{
		private readonly AppDbContext _context;

		public ApiController(AppDbContext context) 
		{ 
			_context = context;
		}

		[HttpGet("GetAllCharacters")]
		public async Task<ActionResult<IEnumerable<Character>>> GetAllCharacters()
		{
			var characters = await _context.Characters.Include(c => c.Class).ToListAsync();
			return Ok(characters); 
		}

		[HttpGet("GetAllCharacterClasses")]
		public async Task<ActionResult<IEnumerable<CharacterClass>>> GetAllCharacterClasses()
		{
			var characters = await _context.CharacterClasses.ToListAsync();
			return Ok(characters);
		}

		[HttpGet("GetChangeHistory")]
		public async Task<ActionResult<IEnumerable<ChangeHistory>>> GetChangeHistory()
		{
			var changes = await _context.ChangeHistories.ToListAsync();
			return Ok(changes);
		}

		[HttpDelete("DeleteCharacterClass/{id}")]
		public async Task<IActionResult> DeleteCharacterClass(int id)
		{
			var characterClass = await _context.CharacterClasses.FindAsync(id);
			if (characterClass == null)
			{
				return NotFound("Класс персонажа не найден.");
			}

			_context.CharacterClasses.Remove(characterClass);
			await _context.SaveChangesAsync();
			AddChangeHistory("Remove", "Class", characterClass.Id, "", "", "");


			return Ok();
		}

		[HttpDelete("DeleteCharacter/{id}")]
		public async Task<IActionResult> DeleteCharacter(int id)
		{
			var character = await _context.Characters.FindAsync(id);
			if (character == null)
			{
				return NotFound("Персонаж не найден");
			}

			_context.Characters.Remove(character);
			await _context.SaveChangesAsync();
			AddChangeHistory("Remove", "Character", character.Id, "", "", "");

			return Ok();
		}

		[HttpPost("CreateCharacter")]
		public IActionResult CreateCharacter([FromBody] CharacterDTO characterDto)
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

				AddChangeHistory("Create", "Character", character.Id, "", character.Name, nameof(character.Name));
				AddChangeHistory("Create", "Character", character.Id, "", character.Level.ToString(), nameof(character.Level));
				AddChangeHistory("Create", "Character", character.Id, "", character.Experience.ToString(), nameof(character.Experience));
				AddChangeHistory("Create", "Character", character.Id, "", character.ClassId.ToString(), nameof(character.ClassId));
			}
			catch (Exception ex)
			{
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

		[HttpPost("EditCharacter")]
		public IActionResult EditCharacter([FromBody] EditCharacterDto editCharacterDto)
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
					error = "Персонаж не найден"
				});
			}

			if (existingCharacter.Name != editCharacterDto.Name)
				AddChangeHistory("Update", "Character", existingCharacter.Id, existingCharacter.Name, editCharacterDto.Name, nameof(editCharacterDto.Name));

			if (existingCharacter.Level != editCharacterDto.Level)
				AddChangeHistory("Update", "Character", existingCharacter.Id, existingCharacter.Level.ToString(), editCharacterDto.Level.ToString(), nameof(editCharacterDto.Level));

			if (existingCharacter.Experience != editCharacterDto.Experience)
				AddChangeHistory("Update", "Character", existingCharacter.Id, existingCharacter.Experience.ToString(), editCharacterDto.Experience.ToString(), nameof(editCharacterDto.Experience));

			if (existingCharacter.ClassId != editCharacterDto.ClassId)
				AddChangeHistory("Update", "Character", existingCharacter.Id, existingCharacter.ClassId.ToString(), editCharacterDto.ClassId.ToString(), nameof(editCharacterDto.ClassId));

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
				return Json(new
				{
					success = false,
					error = "Произошла ошибка при сохранении изменений"
				});
			}

			return Ok();
		}

		[HttpPost("CreateCharacterClass")]
		public IActionResult CreateCharacerClass([FromBody] CharacterClass characterClass)
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

			try
			{
				_context.CharacterClasses.Add(characterClass);
				_context.SaveChanges();

				AddChangeHistory("Create", "Class", characterClass.Id, "", characterClass.Name, nameof(characterClass.Name));
				AddChangeHistory("Create", "Class", characterClass.Id, "", characterClass.Description.ToString(), nameof(characterClass.Description));
				AddChangeHistory("Create", "Class", characterClass.Id, "", characterClass.BaseDamage.ToString(), nameof(characterClass.BaseDamage));
				AddChangeHistory("Create", "Class", characterClass.Id, "", characterClass.BaseHealth.ToString(), nameof(characterClass.BaseHealth));
			}
			catch (Exception ex)
			{
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

		[HttpPost("EditCharacterClass")]
		public IActionResult EdEditCharacterClassit([FromBody]CharacterClass characterClass)
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
				AddChangeHistory("Update", "Class", existingClass.Id, existingClass.Name, characterClass.Name, nameof(characterClass.Name));

			if (existingClass.Description != characterClass.Description)
				AddChangeHistory("Update", "Class", existingClass.Id, existingClass.Description.ToString(), characterClass.Description.ToString(), nameof(characterClass.Description));

			if (existingClass.BaseDamage != characterClass.BaseDamage)
				AddChangeHistory("Update", "Class", existingClass.Id, existingClass.BaseDamage.ToString(), characterClass.BaseDamage.ToString(), nameof(characterClass.BaseDamage));

			if (existingClass.BaseHealth != characterClass.BaseHealth)
				AddChangeHistory("Update", "Class", existingClass.Id, existingClass.BaseHealth.ToString(), characterClass.BaseHealth.ToString(), nameof(characterClass.BaseHealth));


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
				return Json(new
				{
					success = false,
					error = "Произошла ошибка при сохранении изменений"
				});
			}

			return Ok();
		}

		private void AddChangeHistory(string changeType, string entityName, int entityId, string oldValue, string newValue, string propertyName)
		{
			var changeHistory = new ChangeHistory
			{
				ChangeType = changeType,
				EntityName = entityName,
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
