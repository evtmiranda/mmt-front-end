namespace marmitex.Controllers
{
    using System.Web.Mvc;

    public class ResumoPedidoController : Controller
    {
        // GET: ResumoPedido
        public ActionResult Index()
        {
            if (Session["Carrinho"] == null)
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}