namespace marmitex.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Net;
    using System;
    using Newtonsoft.Json;
    using ClassesMarmitex;
    using System.Linq;

    public class HomeController : BaseLoginController
    {
        //private Requisicoes requisicoes;
        private DadosRequisicaoRest retornoRequest;
        private RequisicoesREST rest;
        private Loja loja;
        private List<Brinde> listaBrindes;
        private List<MenuCardapio> listaMenuCardapio;
        private List<Produto> produtos;
        private DadosHorarioEntrega dadosDiasFuncionamento;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HomeController(RequisicoesREST rest)
        {
            this.rest = rest;
            this.listaMenuCardapio = new List<MenuCardapio>();
            this.produtos = new List<Produto>();
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

            //limpa a sessão de dia ativo
            Session["DiaAtivo"] = null;

            //carrega a tela com os cardápios e produtos
            try
            {

                #region busca os dados da loja

                string urlPostLoja = string.Format("/Loja/BuscarLoja/{0}", dominioLoja);

                //busca o id da loja
                retornoRequest = rest.Get(urlPostLoja);

                //verifica se a loja foi encontrada
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception();

                loja = JsonConvert.DeserializeObject<Loja>(retornoRequest.objeto.ToString());

                #endregion

                #region busca os cardápios

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/menucardapio/listar/" + loja.Id);

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaMenuCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

                //view bag com os cardápios
                ViewBag.MenuCardapio = listaMenuCardapio;

                #endregion

                #region monta a lista de produtos

                foreach (var menuCardapio in listaMenuCardapio)
                {
                    foreach (var produto in menuCardapio.Produtos)
                    {
                        produtos.Add(produto);
                    }
                }

                //sessão com os produtos
                Session["Produtos"] = produtos;

                #endregion

                #region busca os dias de funcionamento para verificar se a loja está aberta hj

                //busca os dias de funcionamento
                retornoRequest = rest.Get("/HorarioEntrega/listar/" + loja.Id);

                string jsonDiasFuncionamento = retornoRequest.objeto.ToString();

                dadosDiasFuncionamento = JsonConvert.DeserializeObject<DadosHorarioEntrega>(jsonDiasFuncionamento);

                int diaAtivo = dadosDiasFuncionamento.DiasDeFuncionamento.FindAll(p => p.DiaDisponivel).Count;

                Session["DiaAtivo"] = diaAtivo > 0 ? true : false;

                #endregion

                #region busca o brinde

                //se o usuário estiver logado, o brinde é exibido no menu lateral
                if (Session["usuarioLogado"] != null)
                {
                    UsuarioParceiro usuarioLogado = new UsuarioParceiro();
                    usuarioLogado = (UsuarioParceiro)Session["usuarioLogado"];

                    usuarioLogado.IdLoja = loja.Id;

                    //busca todos os cardápios da loja
                    retornoRequest = rest.Get(string.Format("/Brinde/ListarPorParceiro/{0}/{1}", usuarioLogado.IdParceiro, usuarioLogado.IdLoja));

                    string jsonBrinde = retornoRequest.objeto.ToString();

                    listaBrindes = JsonConvert.DeserializeObject<List<Brinde>>(jsonBrinde);

                    Session["Brinde"] = listaBrindes.Where(p => p.Ativo).FirstOrDefault();
                }

                #endregion

                Session["ExibirBotãoFinalizarPedido"] = true;

            }
            catch (Exception ex)
            {
                ViewBag.MenuCardapioMensagem = "Ocorreu um problema ao buscar o cardápio. Por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                return RedirectToAction("Index", "Erro");
            }

            return View();
        }

    }
}