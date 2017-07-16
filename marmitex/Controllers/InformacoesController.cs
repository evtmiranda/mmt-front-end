using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ClassesMarmitex;
using Newtonsoft.Json;

namespace marmitex.Controllers
{
    public class InformacoesController : Controller
    {
        private Loja loja;
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoGet;
        private List<DadosHorarioEntrega> listaHorarioEntrega;
        private DadosHorarioEntrega horarioEntrega;

        //construtor do controller recebe um RequisicoesREST
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public InformacoesController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: Informacoes
        public ActionResult Index()
        {
            try
            {
                #region busca os dados da loja

                string dominioLoja = Session["dominioLoja"].ToString();

                //guarda apenas o dominio do cliente. por exemplo,
                //o dominio do cliente vem como teste.tasaindo.com.br e vira teste.
                dominioLoja = dominioLoja.Split('.')[0];

                string urlPostLoja = string.Format("/Loja/BuscarLoja/{0}", dominioLoja);

                //busca o id da loja
                retornoGet = rest.Get(urlPostLoja);

                //verifica se a loja foi encontrada
                if (retornoGet.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception();

                loja = JsonConvert.DeserializeObject<Loja>(retornoGet.objeto.ToString());

                #endregion

                #region busca os horários de entrega

                retornoGet = new DadosRequisicaoRest();

                retornoGet = rest.Get("/HorarioEntrega/listar/" + loja.Id);

                if (retornoGet.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemDetalhesPedido = "ocorreu um problema ao buscar os horários de entrega. por favor, tente atualizar a página ou acessar dentro de alguns minutos...";
                    return View("Index");
                }

                string jsonHorariosEntrega = retornoGet.objeto.ToString();

                horarioEntrega = JsonConvert.DeserializeObject<DadosHorarioEntrega>(jsonHorariosEntrega);

                listaHorarioEntrega = new List<DadosHorarioEntrega>
                {
                    horarioEntrega
                };

                Session["HorariosEntrega"] = listaHorarioEntrega;
                ViewBag.HorariosEntrega = listaHorarioEntrega;

                #endregion
            }
            catch (Exception)
            {

                throw;
            }


            return View();
        }
    }
}