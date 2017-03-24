

function adicionarAoCarrinho(Produto) {
    //$post("/Carrinho/AdicionarProduto", { produto: Produto });
    alert("oi");
}

function fecharCarrinho() {
    $post("/Carrinho/FecharCarrinho");
}

function PostAdicionarProduto(url, prod) {
    //após adicionar um produto, chama o método para atualizar
    //a visualização do carrinho
    $.post(url, { produtoJson: prod }, function () {
        AtualizarVisualizacaoDiv('/Carrinho/AtualizarVisualizacaoViewParcial/_CarrinhoCompra', '#visualizacaoCarrinho');
    });
}

function PostRemoverProduto(url, prod) {
    //após remover um produto, chama o método para atualizar
    //a visualização da tela de edição do carrinho
    $.post(url, { produtoJson: prod }, function () {
        AtualizarVisualizacaoDiv('/Carrinho/AtualizarVisualizacaoView/EditarPedido', '#visualizacaoEdicaoCarrinho');
    });
}

function AtualizarVisualizacaoDiv(recurso, idDiv) {
    $.ajax(
        {
            type: 'GET',
            url: recurso,
            dataType: 'html',
            cache: false,
            async: false,
            success: function (data) {
                $(idDiv).html(data);
            }
        });
}

function EsconderDiv(classEsconder, classExibir) {
    //esconde views
    var listaDivsEsconder = document.getElementsByClassName(classEsconder);

    for (i = 0; i < listaDivsEsconder.length; i++) {
        listaDivsEsconder[i].classList.add("escondeDiv");
        listaDivsEsconder[i].classList.remove("exibeDiv");
    }

    //exibe views
    var listaDivsExibir = document.getElementsByClassName(classExibir);

    for (i = 0; i < listaDivsExibir.length; i++) {
        listaDivsExibir[i].classList.remove("escondeDiv");
        listaDivsExibir[i].classList.add("exibeDiv");
    }

}