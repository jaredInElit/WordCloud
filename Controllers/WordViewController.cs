using Microsoft.AspNetCore.Mvc;

namespace WordCloud.Controllers
{
    public class WordViewController : Controller
    {
        public IActionResult WordCloud()
        {
            return View();
        }
    }
}