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

        /// <summary>
        /// Cria um novo produto pedido e adiciona na lista de produtos do pedido
        /// </summary>
        /// <param name="dadosJson"></param>
        public void AdicionarProduto(string dadosJson)
        {
            listaProdutoPedido = new List<ProdutoPedido>();

            //verifica se a sessão de pedidos está preenchida. Se estiver, popula a listaProdutoPedido
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
                listaProdutoPedido.Find(p => p.Produto.Id == produto.Id).ValorTotal += produto.Valor;
            }
            //adiciona o produto se ele ainda não existir no carrinho
            else
            {
                ProdutoPedido prodPedido = new ProdutoPedido
                {
                    Produto = produto,
                    Quantidade = 1,
                    ValorTotal = produto.Valor
                };

                //cria um id temporário para o produto pedido
                Random rnd = new Random();
                prodPedido.Id = rnd.Next();

                //se já existir um produto pedido com o id gerado, gera um novo id para o produto atual antes de adicioná-lo a lista
                while (true)
                {
                    if (listaProdutoPedido.Where(p => p.Id == prodPedido.Id).Count() > 0)
                    {
                        prodPedido.Id = rnd.Next();
                    }
                    else
                        break;
                }

                listaProdutoPedido.Add(prodPedido);
            }

            //atualiza a sessão
            Session["Carrinho"] = listaProdutoPedido;
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

            //adiciona o produto na sessão
            Session["ProdutoComProdutoAdicional"] = produto;
        }

        /// <summary>
        /// Atualiza o produto adicional de um produto e, se necessário, adiciona ao carrinho
        /// </summary>
        /// <param name="adicionalProdutoJson">dados do produto adicional a ser atualizado</param>
        /// <param name="adicionarAoCarrinho">indica se o produto deve ser adicionado ao carrinho</param>
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

                //cria um id temporário para o produto pedido
                Random rnd = new Random();
                prodPedido.Id = rnd.Next();

                //se já existir um produto pedido com o id gerado, gera um novo id para o produto atual antes de adicioná-lo a lista
                while (true)
                {
                    if (listaProdutoPedido.Where(p => p.Id == prodPedido.Id).Count() > 0)
                    {
                        prodPedido.Id = rnd.Next();
                    }
                    else
                        break;
                }

                //calcula o valor total deste produto
                prodPedido.ValorTotal = produto.Valor;
                foreach (var adicional in produto.DadosAdicionaisProdutos)
                {
                    foreach (var itemAdicional in adicional.ItensAdicionais)
                    {
                        if (itemAdicional.Qtd > 0)
                            if (itemAdicional.Valor > 0)
                                prodPedido.ValorTotal += itemAdicional.Qtd * itemAdicional.Valor;
                    }
                }

                listaProdutoPedido.Add(prodPedido);


                Session["Carrinho"] = listaProdutoPedido;
            }

            //se for a adição do último produto adicional, limpa a sessão
            if (adicionarAoCarrinho)
                Session["ProdutoComProdutoAdicional"] = null;
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
                listaProdutoEditar.Where(p => p.Produto.Id == dadosProdAtualizar.idProduto).SingleOrDefault().ValorTotal =
                    dadosProdAtualizar.QuantidadeAtualizada * listaProdutoEditar.Where(p => p.Produto.Id == dadosProdAtualizar.idProduto).SingleOrDefault().Produto.Valor;
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

            //remove o produtoPedido da lista de pedidos
            listaProdutoRemover.Remove(listaProdutoRemover.Where(p => p.Id == produto.Id).SingleOrDefault());

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



    }
}
