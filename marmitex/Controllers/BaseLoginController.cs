using System.Web.Mvc;

namespace marmitex.Controllers
{
    public class BaseLoginController : Controller
    {
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