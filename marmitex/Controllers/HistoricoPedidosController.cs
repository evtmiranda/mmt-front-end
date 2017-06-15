using ClassesMarmitex;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System;
using marmitex.Utils;

namespace marmitex.Controllers
{
    public class HistoricoPedidosController : BaseController
    {
        private RequisicoesREST rest;
        private UsuarioParceiro usuarioLogado;
        private DadosRequisicaoRest retornoRequest;
        private List<Pedido> listaHistoricoPedido;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HistoricoPedidosController(RequisicoesREST rest)
        {
            this.rest = rest;
            this.listaHistoricoPedido = new List<Pedido>();
        }

        // GET: HistoricoPedidos
        public ActionResult Index()
        {
            //valida se o usuário está logado
            //se não estiver, direciona para a tela de login
            if (Session["usuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //cria um usuário com a sessão existente
            usuarioLogado = (UsuarioParceiro)Session["usuarioLogado"];

            //busca os pedidos do cliente
            retornoRequest = rest.Get(string.Format("/Pedido/BuscarHistorico/{0}/{1}", usuarioLogado.Id, usuarioLogado.IdLoja));
            
            //se não encontrar pedidos para este cliente
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
            {
                ViewBag.MensagemHistoricoPedido = "você ainda não realizou pedidos =/";
                //ViewBag.MensagemHistoricoPedido = retornoRequest.objeto.ToString();
                return View("Index");
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemHistoricoPedido = "ocorreu um problema ao consultar os pedidos. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                return View("Index");
            }

            string jsonHistoricoPedido = retornoRequest.objeto.ToString();

            listaHistoricoPedido = JsonConvert.DeserializeObject<List<Pedido>>(jsonHistoricoPedido);

            return View(listaHistoricoPedido);
        }

        [MyErrorHandlerAttribute]
        public ActionResult CancelarPedido(string jsonDadosCancelamento)
        {
            #region valida usuário logado
            
            //valida se o usuário está logado
            //se não estiver, direciona para a tela de login
            if (Session["usuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //cria um usuário com a sessão existente
            usuarioLogado = (UsuarioParceiro)Session["usuarioLogado"];

            #endregion

            #region limpa as viewbags de mensagem

            //ViewBag.MensagemCancelamentoSucesso = null;
            //ViewBag.MensagemCancelamentoAviso = null;

            #endregion

            DadosCancelamento dadosCancelamento = new DadosCancelamento();
            dadosCancelamento = JsonConvert.DeserializeObject<DadosCancelamento>(jsonDadosCancelamento);
            string jsonRetorno;

            bool cancelamentoPermitido = false;

            //verifica se o tempo minimo de antecedencia permite cancelar o pedido
            retornoRequest = rest.Get(string.Format("/HorarioEntrega/TempoAntecedenciaCancelamento/{0}", usuarioLogado.IdLoja));
            jsonRetorno = retornoRequest.objeto.ToString();

            TempoAntecedenciaCancelamentoEntrega tempoPermitidoCancelamento = new TempoAntecedenciaCancelamentoEntrega();
            tempoPermitidoCancelamento = JsonConvert.DeserializeObject<TempoAntecedenciaCancelamentoEntrega>(jsonRetorno);

            //verifica se é permitido cancelar o pedido
            retornoRequest = rest.Get(string.Format("/HorarioEntrega/TempoAntecedenciaCancelamento/PermitirCancelamento/{0}/{1}", dadosCancelamento.IdPedido, usuarioLogado.IdLoja));
            jsonRetorno = retornoRequest.objeto.ToString();

            cancelamentoPermitido = JsonConvert.DeserializeObject<bool>(jsonRetorno);

            //se sim
            if (cancelamentoPermitido)
            {
                //cancela o pedido
                retornoRequest = rest.Post(string.Format("/Pedido/CancelarPedido/{0}/{1}/{2}", dadosCancelamento.IdPedido, usuarioLogado.IdLoja, dadosCancelamento.MotivoCancelamento));

                if(retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    return Json(new { success = false, mensagem = "Não foi possível cancelar o pedido. Por favor, tente novamente ou entre em contato conosco." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, mensagem = "Pedido cancelado com sucesso!" }, JsonRequestBehavior.AllowGet);
            }
            //se não
            else
            {

                string mensagemRetorno = string.Format("Só é possível cancelar o pedido com {0} minutos de antecedência. " +
                    "Se tiver alguma dúvida, por favor, entre em contato conosco.", tempoPermitidoCancelamento.MinutosAntecedencia);

                return Json(new { success = false, mensagem = mensagemRetorno }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}