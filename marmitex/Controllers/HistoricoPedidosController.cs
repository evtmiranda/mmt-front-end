using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace marmitex.Controllers
{
    public class HistoricoPedidosController : BaseController
    {
        private RequisicoesREST rest;
        private UsuarioParceiro usuarioLogado;
        private DadosRequisicaoRest retornoRequest;
        private List<PedidoCliente> listaHistoricoPedido;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HistoricoPedidosController(RequisicoesREST rest)
        {
            this.rest = rest;
            //recebe o usuário logado
            this.usuarioLogado = (UsuarioParceiro)(Session["UsuarioLogado"]);
        }

        // GET: HistoricoPedidos
        public ActionResult Index()
        {
            //busca os pedidos do cliente
            retornoRequest = rest.Get("/Pedido/BuscarHistorico" + usuarioLogado.IdParceiro);

            //se não encontrar pedidos para este cliente
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NotModified)
            {
                ViewBag.MensagemHistoricoPedido = retornoRequest.objeto;
                return View("Index");
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemHistoricoPedido = "ocorreu um problema ao consultar os pedidos. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                return View("Index");
            }

            string jsonHistoricoPedido = retornoRequest.objeto.ToString();

            listaHistoricoPedido = JsonConvert.DeserializeObject<List<PedidoCliente>>(jsonHistoricoPedido);

            return View(listaHistoricoPedido);
        }
    }
}