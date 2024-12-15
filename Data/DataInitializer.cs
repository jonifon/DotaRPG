using DotaRPG.DbModel;

namespace DotaRPG.Data
{
	public class DataInitializer
	{
		public static void Initialize(AppDbContext context)
		{
			context.Database.EnsureCreated();

			if (context.CharacterClasses.Any())
			{
				return; // База уже заполнена
			}

			var characterClasses = new CharacterClass[]
			{
				new CharacterClass
				{
					Name = "Воин",
					Description = "Мастер ближнего боя",
					BaseDamage = 10,
					BaseHealth = 250
				},
				new CharacterClass
				{
					Name = "Маг",
					Description = "Кидается магическими шарами (очень больно)",
					BaseDamage = 90,
					BaseHealth = 100
				},
				new CharacterClass
				{
					Name = "Лучник",
					Description = "Эксперт по дальнему урону",
					BaseDamage = 30,
					BaseHealth = 150
				}
			};

			context.CharacterClasses.AddRange(characterClasses);
			context.SaveChanges();

			var characters = new Character[]
			{
				new Character
				{
					Name = "Арья",
					Level = 5,
					ClassId = 3, // Лучник
                    Experience = 500
				},
				new Character
				{
					Name = "Геральт",
					Level = 10,
					ClassId = 1, // Воин
                    Experience = 1000
				},
				new Character
				{
					Name = "Лиана",
					Level = 7,
					ClassId = 2, // Маг
                    Experience = 750
				}
			};

			context.Characters.AddRange(characters);

			context.SaveChanges();
		}
	}
}
