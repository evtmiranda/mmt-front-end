namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using System;
    using System.Net;
    using System.Web.Mvc;

    public class UsuarioController : BaseLoginController
    {
        private RequisicoesREST rest;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public UsuarioController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: CadastroUsuario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastrar(UsuarioParceiro usuario)
        {
            //guarda o usuário que está tentando fazer o cadastro
            Session["usuarioCadastro"] = usuario;

            #region validação dos campos do formulário

            if (string.IsNullOrEmpty(usuario.Nome))
            {
                ViewBag.MensagemCadastro = "O preenchimento do nome é obrigatório";
                return View("Index");
            }

            if (string.IsNullOrEmpty(usuario.Email))
            {
                ViewBag.MensagemCadastro = "O preenchimento do e-mail é obrigatório";
                return View("Index");
            }

            if (string.IsNullOrEmpty(usuario.Senha))
            {
                ViewBag.MensagemCadastro = "O preenchimento da senha é obrigatório";
                return View("Index");
            }

            if (string.IsNullOrEmpty(usuario.CodigoParceiro))
            {
                ViewBag.MensagemCadastro = "O preenchimento do código da empresa é obrigatório";
                return View("Index");
            }

            #endregion


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
                string urlPost = string.Format("/usuario/cadastrar/usuarioParceiro/'{0}'", dominioRede);

                retornoAutenticacao = rest.Post(urlPost, usuario);

                //se o usuário for criado, direciona para a tela de login
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Created)
                {
                    return View("Index", "Login");
                }
                else if(retornoAutenticacao.HttpStatusCode != HttpStatusCode.InternalServerError)
                {
                    ViewBag.MensagemCadastro = retornoAutenticacao.objeto.ToString();
                    return View("Index");
                }
                //se ocorrer algum erro no cadastro
                else
                {
                    ViewBag.MensagemCadastro = "empresa não encontrada. por favor, verifique se o código da empresa está correto";
                    return View("Index");
                }
            }
            catch
            {
                ViewBag.MensagemCadastro = "humm, ocorreu um problema inesperado. por favor, tente novamente";
                return View("Index");
            }
        }

        public ActionResult Atualizar()
        {
            return null;
        }

        public ActionResult Excluir()
        {
            return null;
        }
    }
}