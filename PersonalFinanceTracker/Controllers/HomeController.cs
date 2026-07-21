using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Models;
using System.Diagnostics;

namespace PersonalFinanceTracker.Controllers
{
	/// <summary>
	/// Basic site controller for general pages such as the home index and privacy page.
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// Home page.
		/// </summary>
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Privacy information page.
		/// </summary>
		public IActionResult Privacy()
		{
			return View();
		}

		/// <summary>
		/// Error handler action that returns the error view populated with request information for diagnostics.
		/// </summary>
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
