namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using Newtonsoft.Json;
    using System.Net;
    using System.Web.Http;
    using System.Web.Mvc;

    public class ResumoPedidoController : BaseController
    {
        private RequisicoesREST rest;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public ResumoPedidoController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: ResumoPedido
        public ActionResult Index()
        {
            if (Session["Carrinho"] == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        public ActionResult ConfirmarPedido(string dadosJson)
        {
            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoCadastroPedido = new DadosRequisicaoRest();

            //tratamento de erros
            try
            {
                Pedido pedido = JsonConvert.DeserializeObject<Pedido>(dadosJson);

                //monta a url de chamada na api
                string urlPost = string.Format("/pedido/cadastrar");

                //realiza o post passando o pedido no body
                retornoCadastroPedido = rest.Post(urlPost, pedido);

                //se o pedido for cadastrado com sucesso, direciona para a tela home
                if (retornoCadastroPedido.HttpStatusCode == HttpStatusCode.Created)
                {
                    Pedido pedidoConcluido = new Pedido();
                    pedidoConcluido = JsonConvert.DeserializeObject<Pedido>(retornoCadastroPedido.objeto.ToString());

                    //guarda as sessões que devem ser mantidas
                    var urlBase = Session["urlBase"];
                    var usuarioLogado = Session["usuarioLogado"];

                    //limpa todas as sessões
                    Session.Clear();

                    //monta as sessões novamente
                    Session["urlBase"] = urlBase;
                    Session["usuarioLogado"] = usuarioLogado;
                    Session["pedidoConcluido"] = pedidoConcluido;

                    return RedirectToAction("PedidoConcluido", "DetalhesPedido");
                }
                //se o pedido não for cadastrado com sucesso
                else if (retornoCadastroPedido.HttpStatusCode == HttpStatusCode.NotModified)
                {
                    Session["MensagemCadastroPedido"] = JsonConvert.DeserializeObject<HttpError>(retornoCadastroPedido.objeto.ToString()).Message;
                    return RedirectToAction("Index");
                }
                //se ocorrer algum erro inesperado
                else
                {
                    Session["MensagemCadastroPedido"] = "humm, ocorreu um problema inesperado. por favor, vá em histórico de pedidos e verifique se o pedido foi salvo";
                    return RedirectToAction("Index");
                }
            }
            //se ocorrer algum erro inesperado
            catch
            {
                Session["MensagemCadastroPedido"] = "humm, ocorreu um problema inesperado. por favor, vá em histórico de pedidos e verifique se o pedido foi salvo";
                return RedirectToAction("Index");
            }
        }
    }
}