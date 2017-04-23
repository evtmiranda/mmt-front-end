namespace marmitex.Controllers
{
    using ClassesMarmitex;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using Newtonsoft.Json;
    using System;

    public class CarrinhoController : BaseLoginController
    {
        List<ProdutoPedido> listaProdutoPedido;
        List<ProdutoPedido> listaProdutoPedidoCalculada;

        public CarrinhoController() { }

        public void AdicionarProduto(string dadosJson)
        {
            //verifica se a sessão de pedidos está preenchida. Se estiver, popula a listaProdutoPedido
            listaProdutoPedido = new List<ProdutoPedido>();

            if (Session["Carrinho"] != null)
                listaProdutoPedido = (List<ProdutoPedido>)Session["Carrinho"];

            Produto produto = new Produto();

            //verifica se recebeu realmente um produto
            if (dadosJson != null)
                produto = JsonConvert.DeserializeObject<Produto>(dadosJson);
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

        /// <summary>
        /// Atualiza a sessão com a quantidade de produtos definida pelo usuário
        /// </summary>
        /// <param name="dadosJson"></param>
        public void AtualizarQtdProdutos(string dadosJson)
        {
            List<DadosAtualizarProduto> listaProdAtualizar = new List<DadosAtualizarProduto>();

            listaProdAtualizar = JsonConvert.DeserializeObject<List<DadosAtualizarProduto>>(dadosJson);

            //lista para armazenar a sessão
            List<ProdutoPedido> listaProdutoEditar = new List<ProdutoPedido>();
            listaProdutoEditar = (List<ProdutoPedido>)Session["Carrinho"];

            foreach (var dadosProdAtualizar in listaProdAtualizar)
            {
                //atualiza a quantidade do produto
                listaProdutoEditar.Where(p => p.Produto.Id == dadosProdAtualizar.idProduto).SingleOrDefault().Quantidade = dadosProdAtualizar.QuantidadeAtualizada;
            }

            //atualiza a sessão
            Session["Carrinho"] = listaProdutoEditar;
        }

        public void RemoverProduto(string dadosJson)
        {
            ProdutoPedido produto = new ProdutoPedido();

            //verifica se recebeu realmente um produto
            if (dadosJson != null)
                produto = JsonConvert.DeserializeObject<ProdutoPedido>(dadosJson);
            else
                return;

            //lista para armazenar a sessão
            List<ProdutoPedido> listaProdutoRemover = new List<ProdutoPedido>();
            listaProdutoRemover = (List<ProdutoPedido>)Session["Carrinho"];

            //remove o produto
            listaProdutoRemover.Remove(listaProdutoRemover.Where(p => p.Produto.Id == produto.Produto.Id).SingleOrDefault());

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

        /// <summary>
        /// Atualiza o produto adicional de um produto
        /// </summary>
        /// <param name="dadosJson">Objeto com o produto adicional</param>
        public void AtualizarProdutoAdicional(string adicionalProdutoJson, bool adicionarAoCarrinho = false)
        {
            //cria um novo produto
            Produto produto = new Produto();

            //verifica se a sessão "ProdutoComProdutoAdicional" existe
            if (Session["ProdutoComProdutoAdicional"] != null)
                produto = (Produto)Session["ProdutoComProdutoAdicional"];
            else
                return;

            //cria um produto adicional para adicionar ao produto
            DadosProdutoAdicional produtoAdicional = new DadosProdutoAdicional();

            //verifica se recebeu realmente um produto adicional
            if (adicionalProdutoJson != null)
                produtoAdicional = JsonConvert.DeserializeObject<DadosProdutoAdicional>(adicionalProdutoJson);
            else
                return;

            //atualiza a quantidade de itens adicionais do produto adicional em questão
            foreach (var itemProdutoAdicional in produto.DadosAdicionaisProdutos)
            {
                if (itemProdutoAdicional.Id == produtoAdicional.Id)
                    foreach (var itemAdicionalAtualizar in itemProdutoAdicional.ItensAdicionais)
                    {
                        foreach (var itemAdicional in produtoAdicional.ItensAdicionais)
                        {
                            if (itemAdicionalAtualizar.Id == itemAdicional.Id)
                                itemAdicionalAtualizar.Qtd = itemAdicional.Qtd;
                        }
                    }
            }

            //se for o último adicional do produto é necessário adicionar o produto na sessão "carrinho"
            if (adicionarAoCarrinho)
            {
                //verifica se a sessão de pedidos está preenchida. Se estiver, popula a listaProdutoPedido
                listaProdutoPedido = new List<ProdutoPedido>();

                if (Session["Carrinho"] != null)
                    listaProdutoPedido = (List<ProdutoPedido>)Session["Carrinho"];

                ProdutoPedido prodPedido = new ProdutoPedido()
                {
                    Produto = produto,
                    Quantidade = 1
                };

                listaProdutoPedido.Add(prodPedido);

                //atualiza a sessão
                Session["Carrinho"] = listaProdutoPedido;
            }

            //limpa a sessão de produto com produto adicional
            Session["ProdutoComProdutoAdicional"] = null;
        }

        /// <summary>
        /// Cria um Produto e um Produto Adicional com os json's recebidos.
        /// Cria a sessão "ProdutoComProdutoAdicional" com o produto
        /// </summary>
        /// <param name="produtoJson"></param>
        /// <param name="adicionalProdutoJson"></param>
        public void AdicionarProdutoComAdicional(string produtoJson, string adicionalProdutoJson)
        {
            //cria um novo produto
            Produto produto = new Produto();

            //verifica se recebeu realmente um produto
            if (produtoJson != null)
                produto = JsonConvert.DeserializeObject<Produto>(produtoJson);
            else
                return;

            //cria um produto adicional para adicionar ao produto
            DadosProdutoAdicional produtoAdicional = new DadosProdutoAdicional();

            //verifica se recebeu realmente um produto adicional
            if (adicionalProdutoJson != null)
                produtoAdicional = JsonConvert.DeserializeObject<DadosProdutoAdicional>(adicionalProdutoJson);
            else
                return;

            //atualiza a quantidade de itens adicionais do produto adicional em questão
            foreach (var itemProdutoAdicional in produto.DadosAdicionaisProdutos) { 
                if(itemProdutoAdicional.Id == produtoAdicional.Id)
                    foreach (var itemAdicionalAtualizar in itemProdutoAdicional.ItensAdicionais) { 
                        foreach (var itemAdicional in produtoAdicional.ItensAdicionais) { 
                            if(itemAdicionalAtualizar.Id == itemAdicional.Id)
                                itemAdicionalAtualizar.Qtd = itemAdicional.Qtd;
                        }
                    }
            }

            //adiciona o produto na sessão
            Session["ProdutoComProdutoAdicional"] = produto;
        }

    }
}
