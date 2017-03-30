namespace marmitex.Controllers
{
    using System.Web.Mvc;

    public class DetalhesPedidoController : Controller
    {
        // GET: DetalhesPedido
        //Tela onde o consumidor irá escolher o horário de entrega e a forma de pagamento
        public ActionResult Index()
        {
            ViewBag.Carrinho = Session["Carrinho"];

            //validação de sessão de usuário
            if (Session["UsuarioLogado"] == null)
            {
                Session["ControllerDestinoAposLogar"] = "DetalhesPedido";
                return RedirectToAction("Index", "Login");
            }

            return View();
            
        }
    }
}