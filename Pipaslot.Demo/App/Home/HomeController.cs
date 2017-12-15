using Microsoft.AspNetCore.Mvc;

namespace Pipaslot.Demo.App.Home
{
    /// <inheritdoc />
    /// <summary>
    /// Home Page
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
