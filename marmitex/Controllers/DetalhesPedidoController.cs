namespace marmitex.Controllers
{
    using Newtonsoft.Json;
    using System.Web.Mvc;
    using ClassesMarmitex;
    using System;

    public class DetalhesPedidoController : Controller
    {
        // GET: DetalhesPedido
        //Tela onde o consumidor irá escolher o horário de entrega e a forma de pagamento
        public ActionResult Index()
        {
            //se o carrinho estiver vazio direciona para a tela de produtos
            if (Session["Carrinho"] == null)
                return RedirectToAction("Index", "Home");

            ViewBag.Carrinho = Session["Carrinho"];

            //validação de sessão de usuário
            if (Session["UsuarioLogado"] == null)
            {
                Session["ControllerDestinoAposLogar"] = "DetalhesPedido";
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public void AtualizarDetalhesPedido(string dadosJson)
        {
            //monta o objeto detalhesPedido com os dados preenchidos pelo usuário
            DetalhePedido detalhesPedido = new DetalhePedido();
            detalhesPedido = JsonConvert.DeserializeObject<DetalhePedido>(dadosJson);

            //alimenta as sessões com os detalhes do pedido
            Session["HorarioEntrega"] = detalhesPedido.HorarioEntrega;
            Session["FormaPagamento"] = detalhesPedido.FormaPagamento;

            if(Convert.ToDecimal(detalhesPedido.Troco) > 0)
                Session["ValorTroco"] = detalhesPedido.Troco;

            if (!string.IsNullOrEmpty(detalhesPedido.Observacao))
                Session["Observacao"] = detalhesPedido.Observacao;

        }
    }
}