

function adicionarAoCarrinho(Produto) {
    //$post("/Carrinho/AdicionarProduto", { produto: Produto });
    alert("oi");
}

function fecharCarrinho() {
    $post("/Carrinho/FecharCarrinho");
}

function Post(url, prod) {

    alert(prod);
    //$.ajax({
    //    type: "POST",
    //    url: url, // the URL of the controller action method
    //    data: prod, // optional data
    //    success: function (result) {
    //        // do something with result
    //    },
    //    error: function (req, status, error) {
    //        // do something with error   
    //    }
    //});

    $.post(url, { produtoJson: prod });
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