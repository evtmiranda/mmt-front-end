namespace marmitex.Controllers
{
    using System;
    using System.Net;
    using System.Web.Mvc;
    using ClassesMarmitex;
    using Newtonsoft.Json;

    public class EsqueciASenhaController : BaseLoginController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public EsqueciASenhaController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: EsqueciASenha
        public ActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// Cria uma nova senha para o usuário e depois envia por e-mail
        /// </summary>
        /// <param name="email">e-mail do usuário parceiro</param>
        /// <returns></returns>
        public ActionResult EnviarSenhaParaEmail(string email)
        {

            //captura a loja em questão
            Session["dominioLoja"] = PreencherSessaoDominioLoja();

            //se não conseguir capturar a loja, direciona para a tela de login
            if (Session["dominioLoja"] == null)
                return RedirectToAction("Index", "Login");

            string dominioLoja = Session["dominioLoja"].ToString();
            //ClassesMarmitex.Email envioEmail = new Email();

            try
            {
                Usuario usuario = new Usuario
                {
                    Email = email
                };

                UsuarioParceiro usuarioParceiro = new UsuarioParceiro();

                //busca o usuário pelo e-mail
                retornoRequest = rest.Post(string.Format("usuario/buscarPorEmail/{0}/{1}", TipoUsuario.Parceiro, dominioLoja), usuario);

                //verifica se os dados do usuário foram encontrados
                if (retornoRequest.HttpStatusCode == HttpStatusCode.OK)
                {
                    usuarioParceiro = JsonConvert.DeserializeObject<UsuarioParceiro>(retornoRequest.objeto.ToString());

                    //gera uma nova senha
                    string novaSenha = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

                    string urlPost = string.Format("/Usuario/AtualizarUsuarioParceiro");

                    //criptografa a senha do usuário
                    usuarioParceiro.Senha = CriptografiaMD5.GerarHashMd5(novaSenha);

                    usuario.IdLoja = usuarioParceiro.IdLoja;
                    retornoRequest = rest.Post(urlPost, usuarioParceiro);

                    if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                    {
                        ViewBag.MensagemRecuperarSenha = retornoRequest.objeto.ToString();
                        return View("Index");
                    }

                    DadosEnvioEmailUnitario dadosEmail = new DadosEnvioEmailUnitario
                    {
                        From = "naoresponder@tasaindo.com.br",
                        To = usuario.Email,
                        Subject = "Recuperação de senha",
                        Text = string.Format("<h1>Olá {0}</h1> <p>A sua nova senha de acesso é: {1}</p>", usuarioParceiro.Nome, novaSenha)
                    };

                    retornoRequest = rest.Post("Email/EnviarEmailUnitario", dadosEmail);

                    if(retornoRequest.HttpStatusCode == HttpStatusCode.OK)
                        return RedirectToAction("Index", "Login");
                    else
                    {
                        ViewBag.MensagemRecuperarSenha = "não foi possível enviar a senha. por favor, tente novamente";
                        return View("Index");
                    }
                }
                else if (retornoRequest.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.MensagemRecuperarSenha = "não existe um usuário cadastrado com este e-mail. tem certeza que já se cadastrou?";
                    return View("Index");
                }
                else
                {
                    ViewBag.MensagemRecuperarSenha = "não foi possível enviar a senha. por favor, tente novamente";
                    return View("Index");
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemRecuperarSenha = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                return View("Index");
            }
        }
    }
}