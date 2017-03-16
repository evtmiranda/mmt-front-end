function adicionarAoCarrinho(Produto) {
    $post("/Carrinho/AdicionarProduto", { produto: Produto });
}

function fecharCarrinho() {
    $post("/Carrinho/FecharCarrinho");
}