using System.Web.Mvc;
using ClassesMarmitex;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace marmitex.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Usuario usuario)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:29783/api/usuario/autenticarusuario");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(usuario, Formatting.None);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}