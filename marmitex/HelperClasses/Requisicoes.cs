using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace marmitex.HelperClasses
{
    public class Requisicoes
    {
        private RequisicoesREST rest;

        //construtor da classe recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public Requisicoes(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        public List<MenuCardapio> ListarMenuCardapio()
        {
            List<MenuCardapio> listaMenuCardapio;

            HttpRequestMarmitex result = rest.Get("/menucardapio/listar");

            if (result.StatusCode != HttpStatusCode.OK)
                return null;

            string json = result.T.ToString();

            listaMenuCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(json);

            return listaMenuCardapio;
        }

        public List<Produto> ListarProdutos()
        {
            List<Produto> listaProdutos;

            HttpRequestMarmitex result = rest.Get("/produto/listar");

            if (result.StatusCode != HttpStatusCode.OK)
                return null;

            string json = result.T.ToString();

            listaProdutos = JsonConvert.DeserializeObject<List<Produto>>(json);

            return listaProdutos;
        }
    }
}
