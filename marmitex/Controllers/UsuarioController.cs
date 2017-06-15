namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using Newtonsoft.Json;
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
                return View("Index", usuario);
            

            //captura a loja em questão
            string dominioLoja = PreencherSessaoDominioLoja();

            //se não conseguir capturar a rede, direciona para a tela de erro
            if (dominioLoja == null)
                return RedirectToAction("Index", "Erro");

            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();

            //variável para armazenar a senha original
            string senhaSemCrip = null;

            //tratamento de erros
            try
            {
                //monta a url de chamada na api
                string urlPost = "/usuario/cadastrar/usuarioParceiro";

                //variável para armazenar a senha original
                senhaSemCrip = usuario.Senha;

                //criptografa a senha do usuário
                usuario.Senha = CriptografiaMD5.GerarHashMd5(usuario.Senha);

                //realiza o post passando o usuário no body
                retornoAutenticacao = rest.Post(urlPost, usuario);

                //se o usuário for criado, direciona para a tela de login
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Created) {
                    Session["MensagemCadastroSucesso"] = "usuário cadastrado com sucesso. realize o login";
                    return RedirectToAction("Index", "Login", ViewBag.MensagemCadastroSucesso);
                }
                //se o cadastro não for autorizado
                else if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    usuario.Senha = senhaSemCrip;
                    ViewBag.MensagemCadastro = JsonConvert.DeserializeObject<HttpError>(retornoAutenticacao.objeto.ToString()).Message;
                    return View("Index", usuario);
                }
                //se ocorrer algum erro inesperado
                else {
                    usuario.Senha = senhaSemCrip;
                    ViewBag.MensagemCadastro = "humm, ocorreu um problema inesperado. por favor, tente novamente";
                    return View("Index", usuario);
                }
            }
            //se ocorrer algum erro inesperado
            catch
            {
                usuario.Senha = senhaSemCrip;
                ViewBag.MensagemCadastro = "humm, ocorreu um problema inesperado. por favor, tente novamente";
                return View("Index", usuario);
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