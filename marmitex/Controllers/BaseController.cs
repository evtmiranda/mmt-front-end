namespace marmitex.Controllers
{
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        //sempre que uma requisição é feita em uma classe que herda esta, 
        //esse método é executado para validar se o usuário está logado
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Session["UsuarioLogado"] == null) {
                Session["PaginaDestinoAposLogin"] = Request.Url.AbsolutePath;
                filterContext.HttpContext.Response.Redirect("/Login/Index", false);
            }

            if (Session["urlBase"] == null)
                //cria sessão para armazenar a url base
                Session["urlBase"] = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
        }

        /// <summary>
        /// identifica a rede de lojas pela URL
        /// </summary>
        /// <returns></returns>
        public string PreencherSessaoDominioRede()
        {
            //captura o host atual
            return Request.Url.Host.Replace('"', ' ').Trim();
        }
    }
}