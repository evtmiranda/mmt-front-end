
function fecharCarrinho() {
    $post("/Carrinho/FecharCarrinho");
}

//após realizar um post, chama o método para atualizar
//a visualização de uma view parcial
function Post(jsonHeaderPost, jsonBodyPost) {
    var dadosPost = JSON.parse(jsonHeaderPost);

    $.post(dadosPost.Recurso, { dadosJson: jsonBodyPost }, function () {
        AtualizarVisualizacaoDiv(dadosPost.ViewParcial, dadosPost.DivASerAtualizada);
    });
}

//após remover um produto, chama o método para atualizar
//a visualização da tela de edição do carrinho
function PostRemoverProduto(url, prod) {
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