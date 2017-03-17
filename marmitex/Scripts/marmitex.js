function adicionarAoCarrinho(Produto) {
    $post("/Carrinho/AdicionarProduto", { produto: Produto });
}

function fecharCarrinho() {
    $post("/Carrinho/FecharCarrinho");
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