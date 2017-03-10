using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace marmitex.HelperClasses
{
    public class RequisicoesREST
    {
        public HttpStatusCode Post(string recurso, object objeto)
        {
            //faz o post de um objeto em um determinado recurso
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:29783/api/" + recurso);
                request.Method = "POST";
                request.Accept = "application/json";

                string json = JsonConvert.SerializeObject(objeto);

                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                request.GetRequestStream().Write(jsonBytes, 0, jsonBytes.Length);

                request.ContentType = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return response.StatusCode;
            }
            //se for algum erro do protocolo HTTP, captura o retorno HTTP para utilizar no retorno do método
            catch (WebException wEx)
            {
                //cria um webResponse
                var webResponse = wEx.Response as System.Net.HttpWebResponse;

                //verifica se não é erro do protocolo HTTP. Se não for, devolve um InternalServerError
                if (wEx.Status != WebExceptionStatus.ProtocolError)
                    return HttpStatusCode.InternalServerError;

                //Retorna o status HTTP
                return webResponse.StatusCode;
            }
            //Se ocorrer qualquer outra exceção retorna um InternalServerError
            catch (System.Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public HttpRequestMarmitex Get(string recurso, int id)
        {
            HttpRequestMarmitex retorno = new HttpRequestMarmitex();

            //faz o post de um objeto em um determinado recurso
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:29783/api/" + recurso);

                request.Headers.Add("id", id.ToString());

                request.Method = "GET";
                request.Accept = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                retorno.StatusCode = response.StatusCode;

                return retorno;
            }
            //se for algum erro do protocolo HTTP, captura o retorno HTTP para utilizar no retorno do método
            catch (WebException wEx)
            {
                //cria um webResponse
                var webResponse = wEx.Response as HttpWebResponse;

                //verifica se não é erro do protocolo HTTP. Se não for, devolve um InternalServerError
                if (wEx.Status != WebExceptionStatus.ProtocolError) {
                    retorno.StatusCode = HttpStatusCode.InternalServerError;
                    return retorno;
                }

                //Retorna o status HTTP
                retorno.StatusCode = webResponse.StatusCode;
                return retorno;
            }
            //Se ocorrer qualquer outra exceção retorna um InternalServerError
            catch (System.Exception)
            {
                retorno.StatusCode = HttpStatusCode.InternalServerError;
                return retorno;
            }
        }

        public HttpRequestMarmitex Get(string recurso)
        {
            HttpRequestMarmitex retorno = new HttpRequestMarmitex();
            string conteudo;

            //faz o post de um objeto em um determinado recurso
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:29783/api/" + recurso);

                request.Method = "GET";
                request.Accept = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                retorno.StatusCode = response.StatusCode;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    conteudo = reader.ReadToEnd();
                }

                retorno.T = conteudo;

                return retorno;
            }
            //se for algum erro do protocolo HTTP, captura o retorno HTTP para utilizar no retorno do método
            catch (WebException wEx)
            {
                //cria um webResponse
                var webResponse = wEx.Response as HttpWebResponse;

                //verifica se não é erro do protocolo HTTP. Se não for, devolve um InternalServerError
                if (wEx.Status != WebExceptionStatus.ProtocolError)
                {
                    retorno.StatusCode = HttpStatusCode.InternalServerError;
                    return retorno;
                }

                //Retorna o status HTTP
                retorno.StatusCode = webResponse.StatusCode;
                return retorno;
            }
            //Se ocorrer qualquer outra exceção retorna um InternalServerError
            catch (System.Exception)
            {
                retorno.StatusCode = HttpStatusCode.InternalServerError;
                return retorno;
            }
        }
    }
}
