namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using Newtonsoft.Json;

    public class CarrinhoController : Controller
    {
        List<ProdutoPedido> listaProdutoPedido;
        List<ProdutoPedido> listaProdutoPedidoCalculada;

        public CarrinhoController()
        {
            if(listaProdutoPedido == null)
                listaProdutoPedido = new List<ProdutoPedido>();

            System.Web.HttpContext.Current.Session["Carrinho"] = "";
        }

        public void AdicionarProduto(string produtoJson)
        {
            Produto produto = new Produto();

            if (produtoJson != null)
                produto = JsonConvert.DeserializeObject<Produto>(produtoJson);
            else
                return;

            //se já existir na lista este produto, somente a quantidade é atualizada
            if (listaProdutoPedido != null && listaProdutoPedido.Where(p => p.Produto.Id == produto.Id).Count() > 1)
            {
                //incrementa a quantidade do produto
                listaProdutoPedido.Find(p => p.Produto.Id == produto.Id).Quantidade++;
            }
            //adiciona o produto se ele ainda não existir no carrinho
            else {

                ProdutoPedido prodPedido = new ProdutoPedido();

                prodPedido.Produto = produto;
                prodPedido.Quantidade = 1;
                 
                listaProdutoPedido.Add(prodPedido);
            }
        }

        public ActionResult FecharCarrinho()
        {
            listaProdutoPedidoCalculada = new List<ProdutoPedido>();

            //calcula o valor total por produto
            foreach (ProdutoPedido prod in listaProdutoPedido)
            {
                prod.ValorTotal = (prod.Quantidade * prod.Produto.Valor);

                listaProdutoPedidoCalculada.Add(prod);
            }

            //adiciona a lista dos produtos ao carrinho
            System.Web.HttpContext.Current.Session["Carrinho"] = listaProdutoPedidoCalculada;

            //vai para os detalhes do pedido
            return RedirectToAction("Index", "DetalhesPedido");
        }
    }
}
