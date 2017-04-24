namespace marmitex.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Net;
    using System;
    using Newtonsoft.Json;
    using ClassesMarmitex;

    public class HomeController : BaseLoginController
    {
        private Requisicoes requisicoes;
        private RequisicoesREST rest;
        private Loja loja;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HomeController(RequisicoesREST rest, Requisicoes requisicoes)
        {
            this.requisicoes = requisicoes;
            this.rest = rest;
        }

        public ActionResult Index()
        {
            //captura a loja em questão
            //a loja é utilizada para validar se o usuário existe e se pertence a loja onde está tentando logar
            Session["dominioLoja"] = PreencherSessaoDominioLoja();

            //se não conseguir capturar a rede, direciona para a tela de erro
            if (Session["dominioLoja"] == null)
                return RedirectToAction("Index", "Erro");

            string dominioLoja = Session["dominioLoja"].ToString();

            //carrega a tela com os cardápios e produtos
            try
            {
                DadosRequisicaoRest retornoBuscaLoja = new DadosRequisicaoRest();
                string urlPostLoja = string.Format("/Loja/BuscarLoja/{0}", dominioLoja);

                //busca o id da loja
                retornoBuscaLoja = rest.Get(urlPostLoja);

                //verifica se a loja foi encontrada
                if (retornoBuscaLoja.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception();

                loja = JsonConvert.DeserializeObject<Loja>(retornoBuscaLoja.objeto.ToString());

                //view bag com os cardápios
                ViewBag.MenuCardapio = requisicoes.ListarMenuCardapio(loja.Id);

                //cria uma lista de cardápio
                List<MenuCardapio> listaMenuCardapio = (List<MenuCardapio>)ViewBag.MenuCardapio;

                //ViewBag.CardapioTelaHome = listaMenuCardapio.Where(p => p.OrdemExibicao == 1).First().Id;

                //carrega os produtos do cardápio
                List<Produto> produtos = new List<Produto>();

                foreach (var menuCardapio in listaMenuCardapio)
                {
                    foreach (var produto in menuCardapio.Produtos)
                    {
                        produtos.Add(produto);
                    }
                }

                //sessão com os produtos
                Session["Produtos"] = produtos;

            }
            catch (Exception ex)
            {
                ViewBag.MenuCardapioMensagem = "ocorreu um problema ao buscar o cardápio. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                return RedirectToAction("Index", "Erro");
            }

            return View();
        }

    }
}