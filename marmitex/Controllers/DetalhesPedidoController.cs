namespace marmitex.Controllers
{
    using Newtonsoft.Json;
    using System.Web.Mvc;
    using ClassesMarmitex;
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class DetalhesPedidoController : BaseController
    {
        private RequisicoesREST rest;
        private UsuarioParceiro usuarioLogado;
        private DadosRequisicaoRest retornoGet;
        private List<FormaDePagamento> listaFormaPagamento;
        private List<HorarioEntrega> listaHorarioEntrega;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public DetalhesPedidoController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: DetalhesPedido
        //Tela onde o consumidor irá escolher o horário de entrega e a forma de pagamento
        public ActionResult Index()
        {
            //se o carrinho estiver vazio direciona para a tela de produtos
            if (Session["Carrinho"] == null)
                return RedirectToAction("Index", "Home");

            //cria um usuário com a sessão existente
            usuarioLogado = (UsuarioParceiro)Session["usuarioLogado"];

            try
            {
                //alimenta a view bag com os dados do carrinho
                ViewBag.Carrinho = Session["Carrinho"];

                #region busca as formas de pagamento

                retornoGet = new DadosRequisicaoRest();

                retornoGet = rest.Get("/formaPagamento/listar/" + usuarioLogado.IdParceiro);

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

                retornoGet = rest.Get("/HorarioEntrega/listar/" + usuarioLogado.IdParceiro);

                if (retornoGet.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemDetalhesPedido = "ocorreu um problema ao buscar as formas de pagamento. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                    return View("Index");
                }

                string jsonHorariosEntrega = retornoGet.objeto.ToString();

                listaHorarioEntrega = JsonConvert.DeserializeObject<List<HorarioEntrega>>(jsonHorariosEntrega);

                ViewBag.HorariosEntrega = listaHorarioEntrega;

                #endregion
            }
            catch (Exception)
            {
                ViewBag.MensagemDetalhesPedido = "ocorreu um problema ao exibir os dados. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                return View("Index");
            }

            return View();
        }

        public ActionResult PedidoConcluido()
        {
            return View("PedidoConcluido");
        }

        public void AtualizarDetalhesPedido(string dadosJson)
        {
            //monta o objeto detalhesPedido com os dados preenchidos pelo usuário
            DetalhePedido detalhesPedido = new DetalhePedido();
            detalhesPedido = JsonConvert.DeserializeObject<DetalhePedido>(dadosJson);

            //calcula o valor total dos produtos
            List<ProdutoPedido> listaProdutosPedido = (List<ProdutoPedido>)Session["Carrinho"];

            for (int i = 0; i < listaProdutosPedido.Count; i++)
            {
                listaProdutosPedido[i].ValorTotal = (listaProdutosPedido[i].Quantidade * listaProdutosPedido[i].Produto.Valor);
            }

            Session["Carrinho"] = listaProdutosPedido;

            //alimenta as sessões com os detalhes do pedido
            Session["HorarioEntrega"] = detalhesPedido.HorarioEntrega;
            Session["FormaPagamento"] = detalhesPedido.FormaPagamento;

            if(!string.IsNullOrEmpty(detalhesPedido.Troco))
                if (Convert.ToDecimal(detalhesPedido.Troco) > 0)
                    Session["ValorTroco"] = detalhesPedido.Troco;

            if (!string.IsNullOrEmpty(detalhesPedido.Observacao))
                Session["Observacao"] = detalhesPedido.Observacao;

            Session["MensagemCamposDetalhesPedido"] = null;
        }
    }
}