using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace marmitex.Controllers
{
    public class AdicionaisController : Controller
    {
        // GET: Adicionais
        public ActionResult Index(int id)
        {

            //Transforma a sessão de produtos em uma lista
            List<ClassesMarmitex.Produto> listaProdutos = (List<ClassesMarmitex.Produto>)Session["Produtos"];

            //Filtra o produto com id recebido via parametro
            ClassesMarmitex.Produto produto = listaProdutos.Where(p => p.Id == id).FirstOrDefault();

            return View(produto);
        }
    }
}