using System.Web.Mvc;

namespace marmitex.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        //Tela inicial com o cardápio
        public ActionResult Index()
        {
            return View();
        }
    }
}