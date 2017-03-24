﻿namespace marmitex.Controllers
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

        public CarrinhoController() { }

        public void AdicionarProduto(string produtoJson)
        {
            //verifica se a sessão de pedidos está preenchida. Se estiver, popula a listaProdutoPedido
            listaProdutoPedido = new List<ProdutoPedido>();

            if (Session["Carrinho"] != null)
                listaProdutoPedido = (List<ProdutoPedido>)Session["Carrinho"];

            Produto produto = new Produto();

            //verifica se recebeu realmente um produto
            if (produtoJson != null)
                produto = JsonConvert.DeserializeObject<Produto>(produtoJson);
            else
                return;

            //se já existir na lista este produto, somente a quantidade é atualizada
            if (listaProdutoPedido.Where(p => p.Produto.Id == produto.Id).Count() > 0)
            {
                //incrementa a quantidade do produto
                listaProdutoPedido.Find(p => p.Produto.Id == produto.Id).Quantidade++;
            }
            //adiciona o produto se ele ainda não existir no carrinho
            else
            {
                ProdutoPedido prodPedido = new ProdutoPedido();

                prodPedido.Produto = produto;
                prodPedido.Quantidade = 1;

                listaProdutoPedido.Add(prodPedido);
            }

            //atualiza a sessão
            Session["Carrinho"] = listaProdutoPedido;
        }

        public void AtualizarQtdProduto(int idProduto, int qtd)
        {
            //lista para armazenar a sessão
            List<ProdutoPedido> listaProdutoEditar = new List<ProdutoPedido>();
            listaProdutoEditar = (List<ProdutoPedido>)Session["Carrinho"];

            //atualiza a quantidade do produto
            listaProdutoEditar.Where(p => p.ID == idProduto).SingleOrDefault().Quantidade = qtd;

            //atualiza a sessão
            Session["Carrinho"] = listaProdutoEditar;
        }

        public void RemoverProduto(string produtoJson)
        {
            Produto produto = new Produto();

            //verifica se recebeu realmente um produto
            if (produtoJson != null)
                produto = JsonConvert.DeserializeObject<Produto>(produtoJson);
            else
                return;

            //lista para armazenar a sessão
            List<ProdutoPedido> listaProdutoRemover = new List<ProdutoPedido>();
            listaProdutoRemover = (List<ProdutoPedido>)Session["Carrinho"];

            //remove o produto
            listaProdutoRemover.Remove(listaProdutoRemover.Where(p => p.Produto.Id == produto.Id).SingleOrDefault());

            //atualiza a sessão
            Session["Carrinho"] = listaProdutoRemover;
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

        public ActionResult EditarPedido()
        {
            return View();
        }

        public PartialViewResult AtualizarVisualizacaoViewParcial(string id)
        {
            string nomeViewParcial = id;

            return PartialView(nomeViewParcial);
        }

        public ViewResult AtualizarVisualizacaoView(string nomeView)
        {
            return View(nomeView);
        }
    }
}
