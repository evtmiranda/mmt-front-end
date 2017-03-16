using System.Web.Mvc;

namespace marmitex.Controllers
{
    public class DetalhesPedidoController : Controller
    {
        // GET: DetalhesPedido
        //Tela onde o consumidor irá escolher o horário de entrega e a forma de pagamento
        public ActionResult Index()
        {
            ViewBag.Carrinho = Session["Carrinho"];

            return View();
        }
    }
}