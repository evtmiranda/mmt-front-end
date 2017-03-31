//function fecharCarrinho() {
//    $post("/Carrinho/FecharCarrinho");
//}

//após realizar um post, chama o método para atualizar
//a visualização de uma view parcial
function Post(jsonHeaderPost, jsonBodyPost) {
    var dadosPost = JSON.parse(jsonHeaderPost);

    $.post(dadosPost.Recurso, { dadosJson: jsonBodyPost }, function () {
        AtualizarVisualizacaoDiv(dadosPost.ViewParcial, dadosPost.DivASerAtualizada);
    });
}

//após realizar um post, faz o redirect para o destino enviado como parâmetro
function PostAtualizarQuantidade(jsonHeaderPost, urlBase, destino) {
    //variáveis para armazenar qual produto deve ter a quantidade atualizada
    // e qual a quantidade
    var produtosHtml = document.getElementsByClassName('js-produtos-atualizar');

    //array para armazenar o id do produto e a quantidade
    var produtos = new Array();

    for (i = 0; i < produtosHtml.length; i++) {
        //objeto para guardar as informações dos produtos
        var atualizarProduto = new Object();
        atualizarProduto.IdProduto = produtosHtml[i].name;
        atualizarProduto.QuantidadeAtualizada = produtosHtml[i].value;

        //adiciona o objeto na lista
        produtos[i] = atualizarProduto;
    }

    //variável com os dados do cabeçalho do post
    var dadosPost = JSON.parse(jsonHeaderPost);
    var atualizarProdutoJson = JSON.stringify(produtos, null, 0);

    $.post(dadosPost.Recurso, { dadosJson: atualizarProdutoJson }, function () {
        Redirecionar(urlBase, destino);
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

function Redirecionar(urlBase, destino) {
    window.location.href = urlBase + destino;
}

function ExibirCampoTroco() {

    var checkDinheiro = document.getElementById("checkDinheiro");

    var divTroco = document.getElementById("divTroco");

    if (checkDinheiro.checked == false) {
        divTroco.setAttribute("style", "display:none");
    }
    else {
        divTroco.setAttribute("style", "display:");
    }
}

//após realizar um post, faz o redirect para o destino enviado como parâmetro
function AvancarParaResumoPedido(jsonHeaderPost, urlBase, destino) {

    //verifica qual o horário de entrega escolhido
    var listaHorarioEntrega = document.getElementsByClassName("radioHorarioEntrega");
    var horarioEntrega;

    //verifica qual a forma de pagamento escolhida
    var listaFormaPagamento = document.getElementsByClassName("checkFormaPagamento");
    var formaPagamento;

    //procura o horário de entrega escolhido
    for (i = 0; i < listaHorarioEntrega.length; i++)
        if (listaHorarioEntrega[i].checked == true)
            horarioEntrega = listaHorarioEntrega[i].value;

    //procura a forma de pagamento escolhida
    for (i = 0; i < listaFormaPagamento.length; i++)
        if (listaFormaPagamento[i].checked == true)
            formaPagamento = listaFormaPagamento[i].value;

    //verifica se os checkbox foram preenchidos
    if (horarioEntrega == null) {
        alert('selecione o horário de entrega');
        return;
    }

    if (formaPagamento == null){
        alert('selecione a forma de pagamento');
        return;
    }

    //monta um objeto com os detalhes do pedido
    var detalhesPedido = new Object();
    detalhesPedido.HorarioEntrega = horarioEntrega;
    detalhesPedido.FormaPagamento = formaPagamento;
    detalhesPedido.Troco = 0;
    detalhesPedido.Observacao = null;

    //se o cliente deseja troco, preenche com o valor
    //se o cliente não precisa de troco o valor irá sempre 0
    var trocoEscolhido = document.getElementById("valorTroco").value;
    if (trocoEscolhido != null && trocoEscolhido > 0) {
        detalhesPedido.Troco = trocoEscolhido;
    }

    //se o cliente escrever alguma observação
    var textoObservacao = document.getElementById("textObservacao").value;
    if (textoObservacao != "") {
        detalhesPedido.Observacao = textoObservacao;
    }

    console.log(jsonHeaderPost);

    //variável com os dados do cabeçalho do post
    var dadosPost = JSON.parse(jsonHeaderPost);
    var detalhesPedidoJson = JSON.stringify(detalhesPedido, null, 0);

    $.post(dadosPost.Recurso, { dadosJson: detalhesPedidoJson }, function () {
        Redirecionar(urlBase, destino);
    });

}