

function adicionarAoCarrinho(Produto) {
    //$post("/Carrinho/AdicionarProduto", { produto: Produto });
    alert("oi");
}

function fecharCarrinho() {
    $post("/Carrinho/FecharCarrinho");
}

function Post(url, prod) {
    //após adicionar um produto, chama o método para atualizar
    //a visualização do carrinho
    $.post(url, { produtoJson: prod }, function () { AtualizarVisualizacaoCarrinho(); });
}

function AtualizarVisualizacaoCarrinho() {
    $.ajax(
    {
        type: 'GET',
        url: '/Carrinho/AtualizarVisualizacaoCarrinho',
        dataType: 'html',
        cache: false,
        async: false,
        success: function (data) {
            $('#visualizacaoCarrinho').html(data);
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