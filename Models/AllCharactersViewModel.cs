using DotaRPG.DbModel;
using System.Reflection;

namespace DotaRPG.Models
{
	public class AllCharactersViewModel
	{
		public List<Character> Characters { get; set; }

		public Dictionary<int, string> CharacterClasses {  get; set; }
	}
}