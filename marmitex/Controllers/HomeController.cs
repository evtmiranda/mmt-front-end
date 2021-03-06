﻿namespace marmitex.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Net;
    using System;
    using Newtonsoft.Json;
    using ClassesMarmitex;
    using System.Linq;
    using marmitex.Utils;

    public class HomeController : BaseLoginController
    {
        //private Requisicoes requisicoes;
        private DadosRequisicaoRest retornoRequest;
        private RequisicoesREST rest;
        private Loja loja;
        private List<Brinde> listaBrindes;
        private DadosBrindeParceiro dadosBrindesParceiro;
        private List<MenuCardapio> listaMenuCardapio;
        private List<Produto> produtos;
        private DadosHorarioEntrega dadosDiasFuncionamento;
        private List<DadosHorarioEntrega> listaHorarioEntrega;
        private SqlServer sqlConn = new SqlServer();

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

            //preenche o nome da loja
            Session["NomeLoja"] = PreencherSessaoDominioLoja();

            #region valida usuário logado

            //se não conseguir capturar a rede, direciona para a tela de erro
            if (Session["dominioLoja"] == null) {
                ViewBag.HomeMensagem = "Não foi possivel identificar a loja";
                return View();
            }

            string dominioLoja = Session["dominioLoja"].ToString();

            //guarda apenas o dominio do cliente. por exemplo,
            //o dominio do cliente vem como teste.tasaindo.com.br e vira teste.
            dominioLoja = dominioLoja.Split('.')[0];
            Session["dominioLoja"] = dominioLoja;

            #endregion

            #region limpa as viewbags de mensagem e sessões

            Session["DiaAtivo"] = null;
            Session["Produtos"] = null;
            Session["Brinde"] = null;
            Session["ExibirBotãoFinalizarPedido"] = null;
            ViewBag.HomeMensagem = null;
            ViewBag.MenuCardapio = null;

            #endregion

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

                //filtra os cardapios ativos
                listaMenuCardapio = listaMenuCardapio.Where(c => c.Ativo).ToList();

                //view bag com os cardápios
                ViewBag.MenuCardapio = listaMenuCardapio;

                #endregion

                #region monta a lista de produtos
                foreach (var menuCardapio in listaMenuCardapio)
                {
                    //filtra os produtos ativos
                    menuCardapio.Produtos = menuCardapio.Produtos.Where(p => p.Ativo).ToList();

                    foreach (var produto in menuCardapio.Produtos)
                    {


                        //filtra os produtos adicionais ativos
                        produto.DadosAdicionaisProdutos = produto.DadosAdicionaisProdutos.Where(pa => pa.Ativo).ToList();

                        //filtra os itens adicionais ativos
                        foreach (var pa in produto.DadosAdicionaisProdutos)
                        {
                            pa.ItensAdicionais = pa.ItensAdicionais.Where(ia => ia.Ativo).ToList();
                        }

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

                listaHorarioEntrega = new List<DadosHorarioEntrega>
                {
                    dadosDiasFuncionamento
                };

                ViewBag.HorariosEntrega = listaHorarioEntrega;

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
                    retornoRequest = rest.Get(string.Format("/BrindeParceiro/ListarPorParceiro/{0}/{1}", usuarioLogado.IdParceiro, usuarioLogado.IdLoja));

                    string jsonBrinde = retornoRequest.objeto.ToString();

                    dadosBrindesParceiro = JsonConvert.DeserializeObject<DadosBrindeParceiro>(jsonBrinde);

                    listaBrindes = dadosBrindesParceiro.Brindes;

                    if(listaBrindes.Where(p => p.Ativo).ToList().Count() > 0)
                        Session["Brinde"] = listaBrindes.Where(p => p.Ativo).ToList();
                }

                #endregion

                Session["ExibirBotãoFinalizarPedido"] = true;

            }
            catch (Exception ex)
            {
                ViewBag.HomeMensagem = "Não foi possivel carregar todas as informações. Por favor, tente novamente ou entre em contato com nosso suporte.";
                return View();
                //return RedirectToAction("Index", "Erro");
            }

            return View();
        }

    }
}