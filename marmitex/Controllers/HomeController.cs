namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;

    public class HomeController : BaseController
    {
        private Requisicoes requisicoes;
        private UsuarioParceiro usuarioLogado;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HomeController(RequisicoesREST rest, Requisicoes requisicoes)
        {
            this.requisicoes = requisicoes;
        }

        public ActionResult Index()
        {
            //cria sessão para armazenar a url base
            Session["urlBase"] = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            //cria um usuário com a sessão existente
            usuarioLogado = (UsuarioParceiro)Session["usuarioLogado"];

            //carrega a tela com os cardápios e produtos
            try
            {
                //view bag com os cardápios
                ViewBag.MenuCardapio = requisicoes.ListarMenuCardapio(usuarioLogado.IdParceiro);

                //busca o cardápio com ordem de exibicao 1 para exibir na tela inicial
                List<MenuCardapio> listaMenuCardapio = (List<MenuCardapio>)ViewBag.MenuCardapio;

                ViewBag.CardapioTelaHome = listaMenuCardapio.Where(p => p.OrdemExibicao == 1).First().Id;

                //carrega os produtos do cardápio
                List<Produto> produtos = new List<Produto>();

                foreach (var menuCardapio in listaMenuCardapio)
                {
                    foreach (var produto in menuCardapio.Produtos)
                    {
                        produtos.Add(produto);
                    }
                }

                //view bag com os produtos
                ViewBag.Produtos = produtos;

            }
            catch (System.Exception ex)
            {
                ViewBag.MenuCardapioMensagem = "ocorreu um problema ao buscar o cardápio. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                return RedirectToAction("Index", "Erro");
            }

            return View();
        }

    }
}