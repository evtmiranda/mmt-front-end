using System.Web.Mvc;
using ClassesMarmitex;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;

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
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:29783/api/usuario/autenticar");
                request.Method = "POST";
                request.Accept = "application/json";

                string json = JsonConvert.SerializeObject(usuario);

                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                request.GetRequestStream().Write(jsonBytes, 0, jsonBytes.Length);

                request.ContentType = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //se o usuário for autenticado, direciona para a tela home
                if (response.StatusCode == HttpStatusCode.Accepted)
                    return RedirectToAction("Index", "Home");

                //retorna para a view 
                return View();
            }
            catch (WebException wEx)
            {
                //verifica se é erro http
                if (wEx.Status != WebExceptionStatus.ProtocolError)
                {
                    ViewBag.MensagemAutenticacao = "Não foi possível realizar o login. Por favor, tente novamente";
                    return View();
                }

                //cria um webResponse
                var webResponse = wEx.Response as System.Net.HttpWebResponse;

                //verifica o status
                if (webResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemAutenticacao = "Usuário não encontrado";
                    return View();
                }
                //se ocorrer algum erro na requisição
                else if (webResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    ViewBag.MensagemAutenticacao = "Não foi possível realizar o login. Por favor, tente novamente";
                    return View();
                }

                return View();

            }
            catch (System.Exception)
            {
                return View();
            }

        }
    }
}