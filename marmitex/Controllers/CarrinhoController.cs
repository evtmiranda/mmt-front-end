namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;

    public class CarrinhoController : Controller
    {
        List<ProdutoPedido> listaProdutoPedido = new List<ProdutoPedido>();
        List<ProdutoPedido> listaProdutoPedidoCalculada = new List<ProdutoPedido>();

        public CarrinhoController()
        {
            Session["Carrinho"] = null;
        }

        public void AdicionarProduto(Produto produto)
        {
            //se já existir na lista este produto, somente a quantidade é atualizada
            if (listaProdutoPedido.Where(p => p.Produto.Id == produto.Id).Count() > 1)
            {
                //incrementa a quantidade do produto
                listaProdutoPedido.Find(p => p.Produto.Id == produto.Id).Quantidade++;
            }
            //adiciona o produto se ele ainda não existir no carrinho
            else
                listaProdutoPedido.Add(new ProdutoPedido { Produto = produto, Quantidade = 1 });
                        
        }

        public ActionResult FecharCarrinho()
        {
            //calcula o valor total por produto
            foreach (ProdutoPedido prod in listaProdutoPedido)
            {
                prod.ValorTotal = (prod.Quantidade * prod.Produto.Valor);

                listaProdutoPedidoCalculada.Add(prod);
            }

            //adiciona a lista dos produtos ao carrinho
            Session["Carrinho"] = listaProdutoPedidoCalculada;

            //vai para os detalhes do pedido
            return RedirectToAction("Index", "DetalhesPedido");
        }
    }
}
