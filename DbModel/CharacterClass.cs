using System.ComponentModel.DataAnnotations;

namespace DotaRPG.DbModel
{
	public class CharacterClass
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Укажите название класса")]
		[StringLength(500, ErrorMessage = "Название не должно быть более 500 символов")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Укажите описание класса")]
		[StringLength(500, ErrorMessage = "Описание не должно быть более 500 символов")]
		public string Description { get; set; }
		[Range(0, 100, ErrorMessage = "Базовый урон должен быть от 0 до 100")]
		public int BaseDamage { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "Базовые ХП не могут быть меньше одного")]
		public int BaseHealth { get; set; }
	}
}
