﻿namespace marmitex.Controllers
{
    using System.Web.Mvc;
    using ClassesMarmitex;
    using System.Net;
    using System;
    using Newtonsoft.Json;

    public class LoginController : BaseLoginController
    {

        private RequisicoesREST rest;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public LoginController(RequisicoesREST rest)
        {
            this.rest = rest;
        }


        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 1. Verifica se o usuário existe na base
        /// 2. Busca os dados do usuário e preenche a sessão "usuarioLogado"
        /// 3. Direciona para a view "Index" do controller "Home"
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Autenticar(Usuario usuario)
        {
            //limpa a sessão com a mensagem de cadastro com sucesso
            Session["MensagemCadastroSucesso"] = null;

            //validação dos campos
            if (!ModelState.IsValid)
            {
                return View("Index", usuario);
            }

            //captura a rede de lojas em questão
            //a rede é utilizada para validar se o usuário existe e se pertence a rede onde está tentando logar
            Session["dominioRede"] = PreencherSessaoDominioRede();

            //se não conseguir capturar a rede, direciona para a tela de erro
            if (Session["dominioRede"] == null)
                return RedirectToAction("Index", "Erro");

            string dominioRede = Session["dominioRede"].ToString();

            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();
            DadosRequisicaoRest retornoDadosUsuario = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/usuario/autenticar/{0}/'{1}'", TipoUsuario.Parceiro, dominioRede);

                retornoAutenticacao = rest.Post(urlPost, usuario);

                //se o usuário for autenticado, direciona para a tela home
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Accepted)
                {
                    UsuarioParceiro usuarioLogado = new UsuarioParceiro();

                    try
                    {
                        //busca os dados do usuário
                        retornoDadosUsuario = rest.Post(string.Format("usuario/buscarPorEmail/{0}", TipoUsuario.Parceiro), usuario);

                        //verifica se os dados do usuário foram encontrados
                        if (retornoDadosUsuario.HttpStatusCode != HttpStatusCode.OK)
                            throw new Exception();

                        usuarioLogado = JsonConvert.DeserializeObject<UsuarioParceiro>(retornoDadosUsuario.objeto.ToString());

                        //armazena o usuário na sessão "usuarioLogado"
                        Session["usuarioLogado"] = usuarioLogado;

                        Session["usuarioLogin"] = null;

                        return RedirectToAction("Index", "Home");
                    }
                    //se não for possível consultar os dados do usuário
                    catch (Exception)
                    {
                        ViewBag.MensagemAutenticacao = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                        return View("Index");
                    }
                }
                else if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemAutenticacao = "usuário ou senha inválida";
                    return View("Index", usuario);
                }

                //se for algum outro erro
                else
                {
                    ViewBag.MensagemAutenticacao = "não foi possível realizar o login. por favor, tente novamente";
                    return View("Index", usuario);
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemAutenticacao = "não foi possível realizar o login. por favor, tente novamente";
                return View("Index", usuario);
            }
        }

        /// <summary>
        /// Limpa todas as sessões e direciona para a tela de login
        /// </summary>
        /// <returns></returns>
        public ActionResult Deslogar()
        {
            //Limpa todas as sessões
            Session.Clear();

            //Direciona para a tela de login
            return View("Index");
        }

    }
}