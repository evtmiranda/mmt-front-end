﻿namespace marmitex.Controllers
{
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        //sempre que uma requisição é feita em uma classe que herda esta, 
        //esse método é executado para validar se o usuário está logado
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Session["UsuarioLogado"] == null)
                filterContext.HttpContext.Response.Redirect("/Default/Index");
        }
    }
}