using DotaRPG.Data;
using DotaRPG.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotaRPG.Controllers
{
	public class ChangeHistoryController : Controller
	{
		private readonly AppDbContext _context;

		public ChangeHistoryController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			ChangeHistoryViewModel model = new ChangeHistoryViewModel()
			{
				changeHistories = _context.ChangeHistories
					.OrderByDescending(c => c.ChangeDate)
					.ToList()
			};

			return View(model);
		}
	}
}
