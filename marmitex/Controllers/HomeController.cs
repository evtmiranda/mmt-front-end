using ClassesMarmitex;
using marmitex.HelperClasses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace marmitex.Controllers
{
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
            //carrega a tela com os cardápios e produtos
            try
            {
                ViewBag.MenuCardapio = requisicoes.ListarMenuCardapio();
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