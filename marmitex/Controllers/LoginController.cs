namespace marmitex.Controllers
{
    using System.Web.Mvc;
    using ClassesMarmitex;
    using System.Net;
    using marmitex.HelperClasses;

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
        public ActionResult Index(Usuario usuario)
        {
            try
            {
                HttpStatusCode result = rest.Post("/usuario/autenticar", usuario);

                //se o usuário for autenticado, direciona para a tela home
                if (result == HttpStatusCode.Accepted)
                {
                    return RedirectToAction("Index", "Home");

                    //armazena o usuário na sessão "Usuário"
                    Session["Usuario"] = usuario;
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemAutenticacao = "Usuário ou senha inválida";
                    return View();
                }

                //se for algum outro erro
                else
                {
                    ViewBag.MensagemAutenticacao = "Não foi possível realizar o login. Por favor, tente novamente";
                    return View();
                }
            }
            catch (System.Exception)
            {
                ViewBag.MensagemAutenticacao = "Não foi possível realizar o login. Por favor, tente novamente";
                return View();
            }
        }
    }
}