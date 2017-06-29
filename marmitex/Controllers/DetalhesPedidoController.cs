namespace marmitex.Controllers
{
    using Newtonsoft.Json;
    using System.Web.Mvc;
    using ClassesMarmitex;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using marmitex.Utils;

    public class DetalhesPedidoController : BaseController
    {
        private RequisicoesREST rest;
        private UsuarioParceiro usuarioLogado;
        private DadosRequisicaoRest retornoGet;
        private List<FormaDePagamento> listaFormaPagamento;
        private List<DadosHorarioEntrega> listaHorarioEntrega;
        private DadosHorarioEntrega horarioEntrega;
        private List<Brinde> listaBrindes;
        private DadosBrindeParceiro dadosBrindesParceiro;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public DetalhesPedidoController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: DetalhesPedido
        //Tela onde o consumidor irá escolher o horário de entrega e a forma de pagamento
        public ActionResult Index()
        {
            Session["ExibirBotãoFinalizarPedido"] = false;

            //se o carrinho estiver vazio direciona para a tela de produtos
            if (Session["Carrinho"] == null)
                return RedirectToAction("Index", "Home");

            //valida se o usuário está logado
            //se não estiver, direciona para a tela de login
            if (Session["usuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //cria um usuário com a sessão existente
            usuarioLogado = (UsuarioParceiro)Session["usuarioLogado"];

            try
            {
                //alimenta a viewbag com os dados do carrinho
                ViewBag.Carrinho = Session["Carrinho"];

                #region busca as formas de pagamento

                retornoGet = new DadosRequisicaoRest();

                retornoGet = rest.Get("/FormaPagamento/listar/" + usuarioLogado.IdLoja);

                if (retornoGet.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemDetalhesPedido = "ocorreu um problema ao buscar as formas de pagamento. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                    return View("Index");
                }

                string jsonFormasPagamento = retornoGet.objeto.ToString();

                listaFormaPagamento = JsonConvert.DeserializeObject<List<FormaDePagamento>>(jsonFormasPagamento);

                ViewBag.FormaPagamento = listaFormaPagamento;

                #endregion

                #region busca os horários de entrega

                retornoGet = new DadosRequisicaoRest();

                retornoGet = rest.Get("/HorarioEntrega/listar/" + usuarioLogado.IdLoja);

                if (retornoGet.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemDetalhesPedido = "ocorreu um problema ao buscar os horários de entrega. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                    return View("Index");
                }

                string jsonHorariosEntrega = retornoGet.objeto.ToString();

                horarioEntrega = JsonConvert.DeserializeObject<DadosHorarioEntrega>(jsonHorariosEntrega);

                listaHorarioEntrega = new List<DadosHorarioEntrega>
                {
                    horarioEntrega
                };

                ViewBag.HorariosEntrega = listaHorarioEntrega;

                #endregion

                #region busca o brinde

                //se o usuário estiver logado, o brinde é exibido no menu lateral
                if (Session["usuarioLogado"] != null)
                {
                    UsuarioParceiro usuarioLogado = new UsuarioParceiro();
                    usuarioLogado = (UsuarioParceiro)Session["usuarioLogado"];

                    //busca todos os cardápios da loja
                    retornoGet = rest.Get(string.Format("/BrindeParceiro/ListarPorParceiro/{0}/{1}", usuarioLogado.IdParceiro, usuarioLogado.IdLoja));

                    string jsonBrinde = retornoGet.objeto.ToString();

                    dadosBrindesParceiro = JsonConvert.DeserializeObject<DadosBrindeParceiro>(jsonBrinde);

                    listaBrindes = dadosBrindesParceiro.Brindes;

                    if (listaBrindes.Where(p => p.Ativo).ToList().Count() > 0)
                        Session["Brinde"] = listaBrindes.Where(p => p.Ativo).ToList();
                }

                #endregion
            }
            catch (Exception ex)
            {
                ViewBag.MensagemDetalhesPedido = "ocorreu um problema ao exibir os dados. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                return View("Index");
            }

            return View();
        }

        public ActionResult PedidoConcluido()
        {
            //se houver algum erro vindo da tela de resumo pedido, volta para ela para exibir a mensagem de erro
            if (Session["MensagemCadastroPedido"] != null)
                return RedirectToAction("Index", "ResumoPedido");

            return View("PedidoConcluido");
        }

        [MyErrorHandler]
        public ActionResult AtualizarDetalhesPedido(string dadosJson)
        {
            //monta o objeto detalhesPedido com os dados preenchidos pelo usuário
            DetalhePedido detalhesPedido = new DetalhePedido();
            detalhesPedido = JsonConvert.DeserializeObject<DetalhePedido>(dadosJson);

            #region validação dos campos

            if(detalhesPedido.FormaPagamento == null || detalhesPedido.FormaPagamento.Count == 0)
                return Json(new { success = false, message = "escolha a forma de pagamento" }, JsonRequestBehavior.AllowGet);

            if(detalhesPedido.HorarioEntrega == null)
                return Json(new { success = false, message = "escolha o horário de entrega" }, JsonRequestBehavior.AllowGet);

            #endregion



            //alimenta as sessões com os detalhes do pedido
            Session["HorarioEntrega"] = detalhesPedido.HorarioEntrega;
            Session["FormaPagamento"] = detalhesPedido.FormaPagamento;
            
            if (!string.IsNullOrEmpty(detalhesPedido.Troco))
                if (Convert.ToDecimal(detalhesPedido.Troco) > 0)
                    Session["ValorTroco"] = detalhesPedido.Troco;
                else
                    Session["ValorTroco"] = null;
            else
                Session["ValorTroco"] = null;

            if (!string.IsNullOrEmpty(detalhesPedido.Observacao))
                Session["Observacao"] = detalhesPedido.Observacao;
            else
                Session["Observacao"] = null;

            Session["MensagemCamposDetalhesPedido"] = null;

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}