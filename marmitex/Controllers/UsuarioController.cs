namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using Newtonsoft.Json;
    using System.Net;
    using System.Web.Http;
    using System.Web.Mvc;
    using System;

    public class UsuarioController : BaseLoginController
    {
        private RequisicoesREST rest;
        private UsuarioParceiro usuarioLogado;
        private DadosRequisicaoRest retornoRequest;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public UsuarioController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: CadastroUsuario
        public ActionResult Index()
        {
            //preenche o nome da loja
            ViewBag.NomeLoja = PreencherSessaoDominioLoja();

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
                string urlPost = "/usuario/cadastrar/usuarioParceiro/" + dominioLoja;

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

        public ActionResult Editar(int id)
        {
            try
            {
                #region validacao usuario logado

                //se a sessão de usuário não estiver preenchida, direciona para a tela de login
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Index", "Login");

                //recebe o usuário logado
                usuarioLogado = (UsuarioParceiro)(Session["UsuarioLogado"]);

                #endregion

                #region limpa as viewbags de mensagem

                ViewBag.MensagemCarregamentoEditarUsuario = null;

                #endregion

                UsuarioParceiro usuarioParceiro = new UsuarioParceiro();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/Usuario/BuscarUsuarioParceiro/" + id);

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarUsuario = retornoRequest.objeto.ToString();
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                usuarioParceiro = JsonConvert.DeserializeObject<UsuarioParceiro>(jsonRetorno);

                //na edição do cadastro o usuário não pode alterar o código da empresa, então o campo não deve ser habilitado.
                ViewBag.NaoExibirCodigoEmpresa = true;

                //limpa a senha do usuário, pois não é possível descriptografar
                usuarioParceiro.Senha = null;

                return View(usuarioParceiro);
            }
            catch (Exception)
            {
                //na edição do cadastro o usuário não pode alterar o código da empresa, então o campo não deve ser habilitado.
                ViewBag.NaoExibirCodigoEmpresa = true;

                ViewBag.MensagemCarregamentoEditarUsuario = "Não foi consultar o usuário. Por favor, tente novamente ou entre em contato com nosso suporte.";
                return View();
            }

        }

        public ActionResult EditarUsuario(UsuarioParceiro usuarioParceiro)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Home");

            //recebe o usuário logado
            usuarioLogado = (UsuarioParceiro)(Session["UsuarioLogado"]);

            #endregion

            #region limpa as viewbags de mensagem

            ViewBag.MensagemEditarUsuario = null;

            #endregion

            #region validação dos campos

            //validação dos campos
            if (!ModelState.IsValid)
            {
                //na edição do cadastro o usuário não pode alterar o código da empresa, então o campo não deve ser habilitado.
                ViewBag.NaoExibirCodigoEmpresa = true;

                return View("Editar", usuarioParceiro);
            }
                

            #endregion

            //variável para armazenar a senha original
            string senhaSemCrip = null;

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/Usuario/AtualizarUsuarioParceiro");

                //variável para armazenar a senha original
                senhaSemCrip = usuarioParceiro.Senha;

                //criptografa a senha do usuário
                usuarioParceiro.Senha = CriptografiaMD5.GerarHashMd5(usuarioParceiro.Senha);

                usuarioParceiro.IdLoja = usuarioLogado.IdLoja;
                retornoRequest = rest.Post(urlPost, usuarioParceiro);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    //na edição do cadastro o usuário não pode alterar o código da empresa, então o campo não deve ser habilitado.
                    ViewBag.NaoExibirCodigoEmpresa = true;

                    ViewBag.MensagemEditarUsuario = retornoRequest.objeto.ToString();
                    return View("Editar", usuarioParceiro);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                //na edição do cadastro o usuário não pode alterar o código da empresa, então o campo não deve ser habilitado.
                ViewBag.NaoExibirCodigoEmpresa = true;

                usuarioParceiro.Senha = senhaSemCrip;
                ViewBag.MensagemEditarUsuario = "Não foi possível atualizar o usuario. Por favor, tente novamente ou entre em contato com nosso suporte.";
                return View("Editar", usuarioParceiro);
            }
        }
    }
}