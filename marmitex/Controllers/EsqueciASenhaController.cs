namespace marmitex.Controllers
{
    using System.Web.Mvc;

    public class EsqueciASenhaController : BaseLoginController
    {
        // GET: EsqueciASenha
        public ActionResult Index()
        {
            return View();
        }
    }
}