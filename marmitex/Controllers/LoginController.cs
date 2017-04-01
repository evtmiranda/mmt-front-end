namespace marmitex.Controllers
{
    using System.Web.Mvc;
    using ClassesMarmitex;
    using System.Net;
    using System;
    using Newtonsoft.Json;

    public class LoginController : Controller
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

        [HttpPost]
        public ActionResult Autenticar(Usuario usuario)
        {
            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();
            DadosRequisicaoRest retornoDadosUsuario = new DadosRequisicaoRest();

            try
            {
                retornoAutenticacao = rest.Post("/usuario/autenticar", usuario);

                //se o usuário for autenticado, direciona para a tela home
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Accepted)
                {
                    Usuario usuarioLogado = new Usuario();
                    try
                    {
                        //busca os dados do usuário
                        retornoDadosUsuario = rest.PostComRetorno("usuario/buscarPorEmail", usuario);

                        //verifica se os dados do usuário foram encontrados
                        if (retornoDadosUsuario.HttpStatusCode != HttpStatusCode.OK)
                            throw new Exception();

                        usuarioLogado = JsonConvert.DeserializeObject<Usuario>(retornoDadosUsuario.objeto.ToString());

                        //armazena o usuário na sessão "Usuário"
                        Session["usuarioLogado"] = usuarioLogado;

                        //verifica qual tela chamou o login
                        if(Session["ControllerDestinoAposLogar"] != null)
                        {
                            string controllerDestinho = (string)Session["ControllerDestinoAposLogar"];

                            Session["ControllerDestinoAposLogar"] = null;

                            return RedirectToAction("Index", controllerDestinho);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    //se não for possível consultar os dados do usuário
                    catch (Exception)
                    {
                        ViewBag.MensagemAutenticacao = "Estamos com dificuldade em buscar dados no servidor. Por favor, tente novamente";
                        return View();
                    }
                }
                else if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemAutenticacao = "Usuário ou senha inválida";
                    return View("Index");
                }

                //se for algum outro erro
                else
                {
                    ViewBag.MensagemAutenticacao = "Não foi possível realizar o login. Por favor, tente novamente";
                    return View("Index");
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemAutenticacao = "Não foi possível realizar o login. Por favor, tente novamente";
                return View("Index");
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