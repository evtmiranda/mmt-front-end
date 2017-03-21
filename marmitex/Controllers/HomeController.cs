namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using marmitex.HelperClasses;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using Newtonsoft.Json;

    public class HomeController : Controller
    {
        private RequisicoesREST rest;
        private Requisicoes requisicoes;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HomeController(RequisicoesREST rest, Requisicoes requisicoes)
        {
            this.rest = rest;
            this.requisicoes = requisicoes;
        }

        public ActionResult Index()
        {

            //JsonConvert.SerializeObject("aa", Formatting.Indented);

            //carrega a tela com os cardápios e produtos
            try
            {
                ViewBag.MenuCardapio = requisicoes.ListarMenuCardapio();
                List<MenuCardapio> listaMenuCardapio = (List<MenuCardapio>)ViewBag.MenuCardapio;

                ViewBag.CardapioTelaHome = listaMenuCardapio.Where(p => p.OrdemExibicao == 1).First().Id;

                ViewBag.Produtos = requisicoes.ListarProdutos();
            }
            catch (System.Exception)
            {
                ViewBag.MenuCardapioMensagem = "Não foi possível consultar o cardápio";
                return RedirectToAction("Index", "Erro");
            }

            return View();
        }

    }
}