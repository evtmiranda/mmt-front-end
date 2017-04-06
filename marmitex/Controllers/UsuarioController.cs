namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Web.Http;
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

        /// <summary>
        /// realiza o cadastro do usuário através do consumo da API de cadastro
        /// </summary>
        /// <param name="usuario">dados do usuário que será cadastrado</param>
        /// <returns></returns>
        public ActionResult Cadastrar(UsuarioParceiro usuario)
        {
            //validação dos campos
            if (!ModelState.IsValid)
            {
                return View("Index", usuario);
            }

            //captura a rede de lojas em questão
            //a rede é utilizada para validar se o usuário existe e se pertence a rede onde está tentando logar
            string dominioRede = PreencherSessaoDominioRede();

            //se não conseguir capturar a rede, direciona para a tela de erro
            if (dominioRede == null)
                return RedirectToAction("Index", "Erro");

            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();

            //tratamento de erros
            try
            {
                //monta a url de chamada na api
                string urlPost = string.Format("/usuario/cadastrar/usuarioParceiro/'{0}'", dominioRede);

                //realiza o post passando o usuário no body
                retornoAutenticacao = rest.Post(urlPost, usuario);

                //se o usuário for criado, direciona para a tela de login
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Created) {
                    Session["MensagemCadastroSucesso"] = "usuário cadastrado com sucesso. realize o login";
                    return RedirectToAction("Index", "Login", ViewBag.MensagemCadastroSucesso);
                }
                //se a empresa não for encontrada pelo código digitado
                else if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.MensagemCadastro = JsonConvert.DeserializeObject<HttpError>(retornoAutenticacao.objeto.ToString()).Message;
                    return View("Index");
                }
                //se já existir um usuário com o e-mail digitado
                else if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemCadastro = JsonConvert.DeserializeObject<HttpError>(retornoAutenticacao.objeto.ToString()).Message;
                    return View("Index");
                }
                //se ocorrer algum erro inesperado
                else { 
                    ViewBag.MensagemCadastro = "humm, ocorreu um problema inesperado. por favor, tente novamente";
                    return View("Index");
                }
            }
            //se ocorrer algum erro inesperado
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