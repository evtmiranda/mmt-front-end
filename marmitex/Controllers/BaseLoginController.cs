using System.Web.Mvc;
using ClassesMarmitex;
using marmitex.Utils;


namespace marmitex.Controllers
{
    public class BaseLoginController : Controller
    {
        SqlServer sqlConn = new SqlServer();

        //sempre que uma requisição é feita em uma classe que herda esta, 
        //esse método é executado para preencher a sessão urlBase
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //if (Session["UsuarioLogado"] == null)
            //    filterContext.HttpContext.Response.Redirect("/Login/Index");

            if (Session["urlBase"] == null)
                //cria sessão para armazenar a url base
                Session["urlBase"] = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
        }

        /// <summary>
        /// identifica a loja pela URL
        /// </summary>
        /// <returns></returns>
        public string PreencherSessaoDominioLoja()
        {
            //captura o host atual
            string host = Request.Url.Host.Replace('"', ' ').Trim();

            host = host.Split('.')[0];

            return host;
        }
    }
}