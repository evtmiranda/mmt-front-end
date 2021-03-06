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

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public LoginController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        public ActionResult Index()
        {
            //preenche o nome da loja
            Session["NomeLoja"] = PreencherSessaoDominioLoja();

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

            //captura a loja em questão
            Session["dominioLoja"] = PreencherSessaoDominioLoja();

            //se não conseguir capturar a loja, direciona para a tela de login
            if (Session["dominioLoja"] == null)
                return RedirectToAction("Index", "Login");

            string dominioLoja = Session["dominioLoja"].ToString();

            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();
            DadosRequisicaoRest retornoDadosUsuario = new DadosRequisicaoRest();

            string senha = null;

            try
            {
                senha = usuario.Senha;
                usuario.Senha = CriptografiaMD5.GerarHashMd5(usuario.Senha);

                string urlPost = string.Format("/usuario/autenticar/{0}/{1}", TipoUsuario.Parceiro, dominioLoja);

                retornoAutenticacao = rest.Post(urlPost, usuario);

                //se o usuário for autenticado, direciona para a tela home
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Accepted)
                {
                    UsuarioParceiro usuarioLogado = new UsuarioParceiro();

                    try
                    {
                        //busca os dados do usuário
                        retornoDadosUsuario = rest.Post(string.Format("usuario/buscarPorEmail/{0}/{1}", TipoUsuario.Parceiro, dominioLoja), usuario);

                        //verifica se os dados do usuário foram encontrados
                        if (retornoDadosUsuario.HttpStatusCode != HttpStatusCode.OK)
                            throw new Exception();

                        usuarioLogado = JsonConvert.DeserializeObject<UsuarioParceiro>(retornoDadosUsuario.objeto.ToString());

                        //armazena o usuário na sessão "usuarioLogado"
                        Session["usuarioLogado"] = usuarioLogado;

                        //limpa a sessão "usuarioLogin"
                        Session["usuarioLogin"] = null;

                        //verifica se foi alguma página específica que chamou a tela de login
                        //se sim, mantém o fluxo de navegação após o login
                        if(Session["PaginaDestinoAposLogin"] != null)
                        {
                            string[] destino = Session["PaginaDestinoAposLogin"].ToString().Split('/');
                            Session["PaginaDestinoAposLogin"] = null;

                            return RedirectToAction(destino[2], destino[1]);
                        }

                        //preenche o nome da loja
                        Session["NomeLoja"] = usuarioLogado.NomeLoja;

                        return RedirectToAction("Index", "Home");
                    }
                    //se não for possível consultar os dados do usuário
                    catch (Exception)
                    {
                        //volta a senha sem criptografia para carregar na tela
                        usuario.Senha = senha;

                        ViewBag.MensagemAutenticacao = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                        return View("Index", usuario);
                    }
                }
                else if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    //volta a senha sem criptografia para carregar na tela
                    usuario.Senha = senha;

                    ViewBag.MensagemAutenticacao = "usuário ou senha inválida";
                    return View("Index", usuario);
                }

                //se for algum outro erro
                else
                {
                    //volta a senha sem criptografia para carregar na tela
                    usuario.Senha = senha;

                    ViewBag.MensagemAutenticacao = "não foi possível realizar o login. por favor, tente novamente";
                    return View("Index", usuario);
                }
            }
            catch (Exception)
            {
                //volta a senha sem criptografia para carregar na tela
                usuario.Senha = senha;

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
            var nomeLoja = Session["NomeLoja"];

            //Limpa todas as sessões
            Session.Clear();

            Session["NomeLoja"] = nomeLoja;

            //Direciona para a tela de login
            return View("Index");
        }

    }
}