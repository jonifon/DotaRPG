using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotaRPG.DbModel
{
	public class Character
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Имя персонажа обязательно")]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Длина имени от 2 до 50 символов")]
		public string Name { get; set; }

		[Range(1, 100, ErrorMessage = "Уровень от 1 до 100")]
		public int Level { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Опыт не может быть отрицательным")]
		public int Experience { get; set; }

		[ForeignKey("Class")]
		public int ClassId { get; set; }

		public CharacterClass Class { get; set; }
	}
}
