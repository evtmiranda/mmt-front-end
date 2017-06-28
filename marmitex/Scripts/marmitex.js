var corConfirmacao = '#bc2026';
var corCancelar = '#f8ac2b';


/**
 * após realizar um post, chama o método para atualizar
  a visualização de uma view parcial
 * @param {any} jsonHeaderPost
 * @param {any} jsonBodyPost
 */
function Post(jsonHeaderPost, jsonBodyPost) {
    var dadosPost = JSON.parse(jsonHeaderPost);

    $.post(dadosPost.Recurso, { dadosJson: jsonBodyPost }, function () {
        AtualizarVisualizacaoDiv(dadosPost.ViewParcial, dadosPost.DivASerAtualizada);
    });
}


/**
 * busca os dados atualizados dos produtos e monta um objeto para atualizar a sessão carrinho com a nova qtd por produto
 * após realizar um post, faz o redirect para o destino enviado como parâmetro
 * @param {any} jsonHeaderPost
 * @param {any} urlBase
 * @param {any} destino
 */
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


/**
 * após remover um produto, chama o método para atualizar
   a visualização da tela de edição do carrinho
 * @param {any} url
 * @param {any} prod
 */
function PostRemoverProduto(url, prod) {
    $.post(url, { produtoJson: prod }, function () {
        AtualizarVisualizacaoDiv('/Carrinho/AtualizarVisualizacaoView/EditarPedido', '#visualizacaoEdicaoCarrinho');
    });
}


/**
 * atualiza o html de uma determinada div
 * @param {any} recurso
 * @param {any} idDiv
 */
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

/**
 * esconde as divs de uma classe e exibe as divs de outra classe
 * @param {any} classEsconder
 * @param {any} classExibir
 */
function EsconderDiv(classEsconder, classExibir) {
    //esconde divs
    var listaDivsEsconder = document.getElementsByClassName(classEsconder);

    for (i = 0; i < listaDivsEsconder.length; i++) {
        listaDivsEsconder[i].classList.add("escondeDiv");
        listaDivsEsconder[i].classList.remove("exibeDiv");
    }

    //exibe divs
    var listaDivsExibir = document.getElementsByClassName(classExibir);

    for (i = 0; i < listaDivsExibir.length; i++) {
        listaDivsExibir[i].classList.remove("escondeDiv");
        listaDivsExibir[i].classList.add("exibeDiv");
    }

}


/** informações do método NavegarModal
 * 1 - captura a quantidade de itens adicionais escolhida pelo cliente
 * 2 - monta um objeto com o id do produto, id do produto adicional e uma lista dos itens adicionais
 * 3 - faz um post do objeto para o recurso 'Carrinho/AtualizarProdutoAdicional' que é responsável por atualizar
       o produto adicional do produto
 * 4 - esconde todas as divs do modal
 * 5 - exibe a div escolhida
 * @param {any} classEsconder
 * @param {any} nomeDivExibir
 * @param {any} classeProdAdicionalAtual
 * @param {any} idProdutoAdicional
 * @param {any} idProduto
 * @param {any} qtdMaxItensAdicional
 */
function NavegarModal(primeiraDivModal, nomeDivExibir, classeProdAdicionalAtual, nomeDivMensagem, idProdutoAdicional, idProduto, ehPrimeiroAdicional, ehUltimoAdicional, qtdMaxItensAdicional, qtdMinItensAdicional, produtoJson) {

    try {
        //nome da classe que deve ter as divs escondidas
        var classEsconder = 'divModal_' + idProduto;

        //limpa a div de mensagem
        document.getElementById(nomeDivMensagem).textContent = "";

        //captura o adicional da div em questão
        var itensAdicionais = document.getElementsByClassName(classeProdAdicionalAtual);

        //busca a quantidade escolhida dos itens do produto adicional desta div
        //array para armazenar os itens
        var itensProdutoAdicional = new Array();

        //variável para armazenar a quantidade de itens adicionais escolhidos
        var qtdItensProdAdicional = Number(0);

        for (var i = 0; i < itensAdicionais.length; i++) {
            itemAdicional = new Object();
            itemAdicional.Id = itensAdicionais[i].getElementsByClassName('idAdicional')[0].value;
            itemAdicional.Qtd = itensAdicionais[i].getElementsByClassName('qtdAdicional')[0].value;

            //se o item vier em branco, é marcado como 0.
            if (itemAdicional.Qtd == "")
                itemAdicional.Qtd = 0;

            itensProdutoAdicional[i] = itemAdicional;

            //vai somando a quantidade escolhida
            qtdItensProdAdicional = qtdItensProdAdicional + Number(itemAdicional.Qtd);
        }

        //monta o produto adicional
        var produtoAdicional = new Object();
        produtoAdicional.Id = idProdutoAdicional;
        produtoAdicional.IdProduto = idProduto;
        produtoAdicional.ItensAdicionais = itensProdutoAdicional;

        //verifica se a quantidade de itens escolhidos é superior ao máximo permitido
        //se sim, exibe uma mensagem ao cliente e não prossegue com o processamento
        if (qtdItensProdAdicional > qtdMaxItensAdicional) {
            document.getElementById(nomeDivMensagem).textContent = 'a quantidade escolhida é maior que o máximo permitido';
            return;
        }

        //verifica se a quantidade de itens escolhidos é menor que o mínimo permitido
        //se sim, exibe uma mensagem ao cliente e não prossegue com o processamento
        if (qtdItensProdAdicional < qtdMinItensAdicional) {
            document.getElementById(nomeDivMensagem).textContent = 'a quantidade escolhida é menor que o mínimo permitido';
            return;
        }

        //dados do produto adicional a ser atualizado
        var produtoAdicionalJson = JSON.stringify(produtoAdicional, null, 0);

        //se for o primeiro e último produto adicional (casos com apenas 1 produto adicional)
        if (ehPrimeiroAdicional && ehUltimoAdicional) {
            console.log("entrou no if certo");

            $.post('/Carrinho/AdicionarProdutoComAdicional', { produtoJson: produtoJson, adicionalProdutoJson: produtoAdicionalJson },

                //faz um post para atualizar o produto adicional do produto e incluir na sessão "carrinho"
                $.post('/Carrinho/AtualizarProdutoAdicional', { adicionalProdutoJson: produtoAdicionalJson, adicionarAoCarrinho: true },
                    function () {
                        var url = "/Carrinho/AtualizarVisualizacaoViewParcial/_CarrinhoCompra/";
                        $("#visualizacaoCarrinho").load(url);

                        var url = "/Carrinho/AtualizarVisualizacaoViewParcial/_MenuCardapio/";
                        $("#visualizacaoCardapio").load(url);

                        //fecha o modal
                        var nomeModal = '#modalProduto_' + idProduto;
                        $(nomeModal).modal('hide');

                        //$('#your-modal-id').modal('hide');
                        $('body').removeClass('modal-open');
                        $('.modal-backdrop').remove();

                        //volta a navegação do modal para o primeiro adicional
                        var listaDivsEsconder = document.getElementsByClassName(classEsconder);

                        for (i = 0; i < listaDivsEsconder.length; i++) {
                            listaDivsEsconder[i].classList.add("escondeDiv");
                            listaDivsEsconder[i].classList.remove("exibeDiv");
                        }

                        //exibe divs
                        var divExibir = document.getElementById(primeiraDivModal);

                        divExibir.classList.remove("escondeDiv");
                        divExibir.classList.add("exibeDiv");

                        //limpa os inputs do modal
                        $(classEsconder)
                            .find("input,textarea,select")
                            .val('')
                            .end()
                            .find("input[type=checkbox], input[type=radio]")
                            .prop("checked", "")
                            .end();
                    }));
        }
        //se for a escolha do primeiro adicional do produto o produto deve ser criado
        //e este adicional adicionado a ele. Os próximos adicionais escolhidos serão adicionados a este produto
        //quando for a escolha do último adicional o produto será incluido na sessão "carrinho"
        else if (ehPrimeiroAdicional) {
            $.post('/Carrinho/AdicionarProdutoComAdicional', { produtoJson: produtoJson, adicionalProdutoJson: produtoAdicionalJson },
                ProximaDivModal(classEsconder, nomeDivExibir));
        }
        //se não for o primeiro nem o último produto adicional
        else if (!ehUltimoAdicional) {
            //faz um post para atualizar o produto adicional do produto
            $.post('/Carrinho/AtualizarProdutoAdicional', { adicionalProdutoJson: produtoAdicionalJson },
                ProximaDivModal(classEsconder, nomeDivExibir));
        }
        //se for o último produto adicional
        else {
            //faz um post para atualizar o produto adicional do produto e incluir na sessão "carrinho"
            $.post('/Carrinho/AtualizarProdutoAdicional', { adicionalProdutoJson: produtoAdicionalJson, adicionarAoCarrinho: true },
                function () {

                    var url = "/Carrinho/AtualizarVisualizacaoViewParcial/_CarrinhoCompra/";
                    $("#visualizacaoCarrinho").load(url);

                    var url = "/Carrinho/AtualizarVisualizacaoViewParcial/_MenuCardapio/";
                    $("#visualizacaoCardapio").load(url);

                    //fecha o modal
                    var nomeModal = '#modalProduto_' + idProduto;
                    $(nomeModal).modal('hide');

                    //$('#your-modal-id').modal('hide');
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();

                    //volta a navegação do modal para o primeiro adicional
                    var listaDivsEsconder = document.getElementsByClassName(classEsconder);

                    for (i = 0; i < listaDivsEsconder.length; i++) {
                        listaDivsEsconder[i].classList.add("escondeDiv");
                        listaDivsEsconder[i].classList.remove("exibeDiv");
                    }

                    //exibe divs
                    var divExibir = document.getElementById(primeiraDivModal);

                    divExibir.classList.remove("escondeDiv");
                    divExibir.classList.add("exibeDiv");

                    //limpa os inputs do modal
                    $(classEsconder)
                        .find("input,textarea,select")
                        .val('')
                        .end()
                        .find("input[type=checkbox], input[type=radio]")
                        .prop("checked", "")
                        .end();

                });
        }
    } catch (e) {
        document.getElementById(nomeDivMensagem).textContent = 'ocorreu um erro. por favor, tente novamente ou entre em contato com o administrador.';
    }

}

function ProximaDivModal(classEsconder, nomeDivExibir) {
    //esconde divs
    var listaDivsEsconder = document.getElementsByClassName(classEsconder);

    for (i = 0; i < listaDivsEsconder.length; i++) {
        listaDivsEsconder[i].classList.add("escondeDiv");
        listaDivsEsconder[i].classList.remove("exibeDiv");
    }

    //exibe divs
    var divExibir = document.getElementById(nomeDivExibir);

    divExibir.classList.remove("escondeDiv");
    divExibir.classList.add("exibeDiv");
}

/**
 * Esconde a div atual e exibe a div anterior do modal
 * @param {any} classEsconder
 * @param {any} nomeDivExibir
 */
function DivAnteriorModal(classEsconder, nomeDivExibir, nomeDivMensagem) {

    try {
        //limpa a div de mensagem
        document.getElementById(nomeDivMensagem).textContent = "";

        //esconde divs
        var listaDivsEsconder = document.getElementsByClassName(classEsconder);

        for (i = 0; i < listaDivsEsconder.length; i++) {
            listaDivsEsconder[i].classList.add("escondeDiv");
            listaDivsEsconder[i].classList.remove("exibeDiv");
        }

        //exibe divs
        var divExibir = document.getElementById(nomeDivExibir);

        divExibir.classList.remove("escondeDiv");
        divExibir.classList.add("exibeDiv");
    }
    catch (e) {
        document.getElementById(nomeDivMensagem).textContent = 'ocorreu um erro. por favor, tente novamente ou entre em contato com o administrador.';
    }

}


/**
 * Redireciona o cliente para um determinado destino
 * @param {any} urlBase
 * @param {any} destino
 */
function Redirecionar(urlBase, destino) {
    window.location.href = urlBase + destino;
}


/**
 * Torna a div para troco visível
 */
function ExibirCampoTroco() {

    var checkDinheiro = document.getElementById("checkDinheiro");

    var divTroco = document.getElementById("divTroco");
    var divValorTroco = document.getElementById("valorTroco");

    if (checkDinheiro.checked == false) {
        divTroco.setAttribute("style", "display:none");
        divValorTroco.value = 0;
    }
    else {
        divTroco.setAttribute("style", "display:");
    }
}


/**
 * 1 - captura os dados preenchidos na tela
 * 2 - valida se os campos obrigatórios estão preenchidos
 * 3 - faz um post dos dados no recurso ''
 * 4 - redireciona o cliente para a tela ''
 * @param {any} jsonHeaderPost
 * @param {any} urlBase
 * @param {any} destino
 */
function AvancarParaResumoPedido(jsonHeaderPost, urlBase, destino) {

    //verifica qual o horário de entrega escolhido
    var listaHorarioEntrega = document.getElementsByClassName("radioHorarioEntrega");
    var horarioEntrega;

    //verifica qual a forma de pagamento escolhida
    var listaFormaPagamento = document.getElementsByClassName("checkFormaPagamento");
    var formaPagamento = new Array();

    //procura o horário de entrega escolhido
    for (i = 0; i < listaHorarioEntrega.length; i++)
        if (listaHorarioEntrega[i].checked == true)
            horarioEntrega = listaHorarioEntrega[i].value;

    //procura a forma de pagamento escolhida
    for (i = 0; i < listaFormaPagamento.length; i++)
        if (listaFormaPagamento[i].checked == true)
            formaPagamento[i] = listaFormaPagamento[i].value;

    //verifica se os checkbox foram preenchidos
    if (horarioEntrega == null) {
        alert('selecione o horário de entrega');
        return;
    }

    if (formaPagamento == null) {
        alert('selecione a forma de pagamento');
        return;
    }

    //monta um objeto com os detalhes do pedido
    var detalhesPedido = new Object();
    detalhesPedido.HorarioEntrega = horarioEntrega;
    detalhesPedido.FormaPagamento = formaPagamento;
    detalhesPedido.Troco = "0";
    detalhesPedido.Observacao = null;

    //se o cliente deseja troco, preenche com o valor
    //se o cliente não precisa de troco o valor irá sempre 0
    var trocoEscolhido = document.getElementById("valorTroco").value;

    if (trocoEscolhido != null && trocoEscolhido != "0") {
        detalhesPedido.Troco = trocoEscolhido;
        console.log(detalhesPedido.Troco);
    }

    //se o cliente escrever alguma observação
    var textoObservacao = document.getElementById("textObservacao").value;
    if (textoObservacao != "") {
        detalhesPedido.Observacao = textoObservacao;
    }

    //variável com os dados do cabeçalho do post
    var dadosPost = JSON.parse(jsonHeaderPost);
    var detalhesPedidoJson = JSON.stringify(detalhesPedido, null, 0);

    $.post(dadosPost.Recurso, { dadosJson: detalhesPedidoJson }, function () {
        Redirecionar(urlBase, destino);
    });

}


/**
 * exibe uma mensagem de que o pedido foi enviado com sucesso
 */
function avisoConfirmacaoPedido() {
    alert('Pedido enviado com sucesso');
    window.location.href = "/DetalhesPedido/PedidoConcluido";
}


function ConfirmarPedido(jsonHeaderPost, jsonBodyPost, urlBase, destino) {
    //variável com os dados do cabeçalho do post
    var dadosPost = JSON.parse(jsonHeaderPost);

    $.post(dadosPost.Recurso, { dadosJson: jsonBodyPost }, function () {
        Redirecionar(urlBase, destino);
    });
}

/**
 * Faz a pesquisa de um produto pelo valor inserido no input
 */
function PesquisarProduto() {
    // Declare variables 
    var input, filter, divMae, divClass, div, i;
    input = document.getElementById("inputPesquisa");
    filter = input.value.toUpperCase();
    divMae = document.getElementsByClassName("menuCardapio");
    divClass = document.getElementsByClassName("nome-produto-pesquisa");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < divMae.length; i++) {
        //for (var j = 0; j < divClass.length; j++) {
        div = divClass[i];
        if (div) {
            if (div.innerHTML.toUpperCase().indexOf(filter) > -1) {
                divMae[i].style.display = "";
            } else {
                divMae[i].style.display = "none";
            }
        }
        //}
    }
}

function CancelarPedido(urlPost, idPedido) {
    swal({
        title: 'Confirma o cancelamento do pedido?',
        html: "Não será possível desfazer a ação",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: corConfirmacao,
        cancelButtonColor: corCancelar,
        cancelButtonText: 'Voltar',
        confirmButtonText: 'Sim, cancelar!'
    }).then(function () {
        swal({
            title: 'Escreva o motivo do cancelamento',
            input: 'text',
            showCancelButton: true,
            confirmButtonColor: corConfirmacao,
            cancelButtonColor: corCancelar,
            cancelButtonText: 'Voltar',
            confirmButtonText: 'Confirmar'
        }).then(function (motivoCancelamento) {

            dadosCancelamento = new Object();
            dadosCancelamento.IdPedido = idPedido;
            dadosCancelamento.MotivoCancelamento = motivoCancelamento;

            var dadosCancelamentoJson = JSON.stringify(dadosCancelamento, null, 0);

            $.getJSON(urlPost, { jsonDadosCancelamento: dadosCancelamentoJson }, function (result) {
                if (!result.success) {
                    if (result.mensagem == "Não foi possível cancelar o pedido. Por favor, tente novamente ou entre em contato conosco.") {
                        swal({
                            title: 'Oops',
                            html: result.mensagem,
                            type: 'error',
                            confirmButtonColor: corConfirmacao,
                            cancelButtonColor: corCancelar,
                        })
                    }
                    else {
                        swal({
                            title: 'Aviso',
                            html: result.mensagem,
                            type: 'warning',
                            confirmButtonColor: corConfirmacao,
                            cancelButtonColor: corCancelar,
                        })
                    }
                }
                else {
                    swal({
                        title: 'Sucesso',
                        html: "Cancelamento realizada com sucesso.",
                        type: 'success',
                        confirmButtonText: 'Ok',
                        confirmButtonColor: corConfirmacao,
                        cancelButtonColor: corCancelar,
                    }).then(function () {
                        //recarrega a pagina
                        window.location.reload();
                    });
                }
            });
        })
    });
}


//mascaras
function formataMascara(campo, evt, formato) {
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;

    var result = "";
    var maskIdx = formato.length - 1;
    var error = false;
    var valor = campo.value;
    var posFinal = false;
    if (campo.setSelectionRange) {
        if (campo.selectionStart == valor.length)
            posFinal = true;
    }
    valor = valor.replace(/[^0123456789Xx]/g, '');
    for (var valIdx = valor.length - 1; valIdx >= 0 && maskIdx >= 0; --maskIdx) {
        var chr = valor.charAt(valIdx);
        var chrMask = formato.charAt(maskIdx);
        switch (chrMask) {
            case '#':
                if (!(/\d/.test(chr)))
                    error = true;
                result = chr + result;
                --valIdx;
                break;
            case '@':
                result = chr + result;
                --valIdx;
                break;
            default:
                result = chrMask + result;
        }
    }
    campo.value = result;
    campo.style.color = error ? 'red' : '';
    if (posFinal) {
        campo.selectionStart = result.length;
        campo.selectionEnd = result.length;
    }
    return result;
}
// Formata o campo valor monetÃ¡rio
function formataValor(campo, evt) {
    //1.000.000,00
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    if (vr.length > 0) {
        vr = parseFloat(vr.toString()).toString();
        tam = vr.length;
        if (tam == 1)
            campo.value = "0.0" + vr;
        if (tam == 2)
            campo.value = "0." + vr;
        if ((tam > 2) && (tam <= 5)) {
            campo.value = vr.substr(0, tam - 2) + '.' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 6) && (tam <= 8)) {
            campo.value = vr.substr(0, tam - 5) + '.' + vr.substr(tam - 5, 3) + '.' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 9) && (tam <= 11)) {
            campo.value = vr.substr(0, tam - 8) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + '.' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 12) && (tam <= 14)) {
            campo.value = vr.substr(0, tam - 11) + '.' + vr.substr(tam - 11, 3) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + '.' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 15) && (tam <= 18)) {
            campo.value = vr.substr(0, tam - 14) + '.' + vr.substr(tam - 14, 3) + '.' + vr.substr(tam - 11, 3) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + '.' + vr.substr(tam - 2, tam);
        }
    }
    MovimentaCursor(campo, xPos);
}
// Formata data no padrÃ£o DD/MM/YYYY
function formataData(campo, evt) {
    var xPos = PosicaoCursor(campo);
    //dd/MM/yyyy
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2 && tam < 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2);
    if (tam == 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/';
    if (tam > 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4);
    MovimentaCursor(campo, xPos);

    /*Valida a Data*/
    day = data.substring(0, 2);
    month = data.substring(3, 5);
    year = data.substring(6, 10);

    if (year <= 1920) {
        alert('Data Nascimento Inválida');
    }

    if ((month == 01) || (month == 03) || (month == 05) || (month == 07) || (month == 08) || (month == 10) || (month == 12)) {//mes com 31 dias
        if ((day < 01) || (day > 31)) {
            alert('Data Nascimento Inválida');
        }
    } else

        if ((month == 04) || (month == 06) || (month == 09) || (month == 11)) {//mes com 30 dias
            if ((day < 01) || (day > 30)) {
                alert('Data Nascimento Inválida');
            }
        } else

            if ((month == 02)) {//February and leap year
                if ((year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0))) {
                    if ((day < 01) || (day > 29)) {
                        alert('Data Nascimento Inválida');
                    }
                } else {
                    if ((day < 01) || (day > 28)) {
                        alert('Data Nascimento Inválida');
                    }
                }
            }

}
//descobre qual a posiÃ§Ã£o do cursor no campo
function PosicaoCursor(textarea) {
    var pos = 0;
    if (typeof (document.selection) != 'undefined') {
        //IE
        var range = document.selection.createRange();
        var i = 0;
        for (i = textarea.value.length; i > 0; i--) {
            if (range.moveStart('character', 1) == 0)
                break;
        }
        pos = i;
    }
    if (typeof (textarea.selectionStart) != 'undefined') {
        //FireFox
        pos = textarea.selectionStart;
    }
    if (pos == textarea.value.length)
        return 0; //retorna 0 quando nÃ£o precisa posicionar o elemento
    else
        return pos; //posiÃ§Ã£o do cursor
}
// move o cursor para a posiÃ§Ã£o pos
function MovimentaCursor(textarea, pos) {
    if (pos <= 0)
        return; //se a posiÃ§Ã£o for 0 nÃ£o reposiciona
    if (typeof (document.selection) != 'undefined') {
        //IE
        var oRange = textarea.createTextRange();
        var LENGTH = 1;
        var STARTINDEX = pos;
        oRange.moveStart("character", -textarea.value.length);
        oRange.moveEnd("character", -textarea.value.length);
        oRange.moveStart("character", pos);
        //oRange.moveEnd("character", pos);
        oRange.select();
        textarea.focus();
    }
    if (typeof (textarea.selectionStart) != 'undefined') {
        //FireFox
        textarea.selectionStart = pos;
        textarea.selectionEnd = pos;
    }
}
//Formata data e hora no padrÃ£o DD/MM/YYYY HH:MM
function formataDataeHora(campo, evt) {
    xPos = PosicaoCursor(campo);
    //dd/MM/yyyy
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2 && tam < 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2);
    if (tam == 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/';
    if (tam > 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4);
    if (tam > 8 && tam < 11)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4, 4) + ' ' + vr.substr(8, 2);
    if (tam >= 11)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4, 4) + ' ' + vr.substr(8, 2) + ':' + vr.substr(10);
    campo.value = campo.value.substr(0, 16);
    //    if(xPos == 2 || xPos == 5)
    //        xPos = xPos +1;
    //    if(xPos == 11 || xPos == 14)
    //        xPos = xPos +2;
    MovimentaCursor(campo, xPos);
}
// Formata sÃ³ nÃºmeros
function formataInteiro(campo, evt) {
    //1234567890
    xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    campo.value = filtraNumeros(filtraCampo(campo));
    MovimentaCursor(campo, xPos);
}
// Formata hora no padrao HH:MM
function formataHora(campo, evt) {
    //HH:mm
    xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    if (tam == 2)
        campo.value = vr.substr(0, 2) + ':';
    if (tam > 2 && tam < 5)
        campo.value = vr.substr(0, 2) + ':' + vr.substr(2);
    //    if(xPos == 2)
    //        xPos = xPos + 1;
    MovimentaCursor(campo, xPos);
}
// limpa todos os caracteres especiais do campo solicitado
function filtraCampo(campo) {
    var s = "";
    var cp = "";
    vr = campo.value;
    tam = vr.length;
    for (i = 0; i < tam; i++) {
        if (vr.substring(i, i + 1) != "/"
            && vr.substring(i, i + 1) != "-"
            && vr.substring(i, i + 1) != "."
            && vr.substring(i, i + 1) != "("
            && vr.substring(i, i + 1) != ")"
            && vr.substring(i, i + 1) != ":"
            && vr.substring(i, i + 1) != ",") {
            s = s + vr.substring(i, i + 1);
        }
    }
    return s;
    //return campo.value.replace("/", "").replace("-", "").replace(".", "").replace(",", "")
}
// limpa todos caracteres que nÃ£o sÃ£o nÃºmeros
function filtraNumeros(campo) {
    var s = "";
    var cp = "";
    vr = campo;
    tam = vr.length;
    for (i = 0; i < tam; i++) {
        if (vr.substring(i, i + 1) == "0" ||
            vr.substring(i, i + 1) == "1" ||
            vr.substring(i, i + 1) == "2" ||
            vr.substring(i, i + 1) == "3" ||
            vr.substring(i, i + 1) == "4" ||
            vr.substring(i, i + 1) == "5" ||
            vr.substring(i, i + 1) == "6" ||
            vr.substring(i, i + 1) == "7" ||
            vr.substring(i, i + 1) == "8" ||
            vr.substring(i, i + 1) == "9") {
            s = s + vr.substring(i, i + 1);
        }
    }
    return s;
    //return campo.value.replace("/", "").replace("-", "").replace(".", "").replace(",", "")
}
// limpa todos caracteres que nÃ£o sÃ£o letras
function filtraCaracteres(campo) {
    vr = campo;
    for (i = 0; i < tam; i++) {
        //Caracter
        if (vr.charCodeAt(i) != 32 && vr.charCodeAt(i) != 94 && (vr.charCodeAt(i) < 65 ||
            (vr.charCodeAt(i) > 90 && vr.charCodeAt(i) < 96) ||
            vr.charCodeAt(i) > 122) && vr.charCodeAt(i) < 192) {
            vr = vr.replace(vr.substr(i, 1), "");
        }
    }
    return vr;
}
// limpa todos caracteres que nÃ£o sÃ£o nÃºmeros, menos a vÃ­rgula
function filtraNumerosComVirgula(campo) {
    var s = "";
    var cp = "";
    vr = campo;
    tam = vr.length;
    var complemento = 0; //flag paga contar o nÃºmero de virgulas
    for (i = 0; i < tam; i++) {
        if ((vr.substring(i, i + 1) == "," && complemento == 0 && s != "") ||
            vr.substring(i, i + 1) == "0" ||
            vr.substring(i, i + 1) == "1" ||
            vr.substring(i, i + 1) == "2" ||
            vr.substring(i, i + 1) == "3" ||
            vr.substring(i, i + 1) == "4" ||
            vr.substring(i, i + 1) == "5" ||
            vr.substring(i, i + 1) == "6" ||
            vr.substring(i, i + 1) == "7" ||
            vr.substring(i, i + 1) == "8" ||
            vr.substring(i, i + 1) == "9") {
            if (vr.substring(i, i + 1) == ",")
                complemento = complemento + 1;
            s = s + vr.substring(i, i + 1);
        }
    }
    return s;
}
function formataMesAno(campo, evt) {
    //MM/yyyy
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2);
    MovimentaCursor(campo, xPos);
}
function formataCNPJ(campo, evt) {
    //99.999.999/9999-99
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2 && tam < 5)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2);
    else if (tam >= 5 && tam < 8)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5);
    else if (tam >= 8 && tam < 12)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8);
    else if (tam >= 12)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8, 4) + '-' + vr.substr(12);
    MovimentaCursor(campo, xPos);
}
function formataCPF(campo, evt) {
    //999.999.999-99
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 3 && tam < 6)
        campo.value = vr.substr(0, 3) + '.' + vr.substr(3);
    else if (tam >= 6 && tam < 9)
        campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6);
    else if (tam >= 9)
        campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6, 3) + '-' + vr.substr(9);
    MovimentaCursor(campo, xPos);
}
function formataDouble(campo, evt) {
    //18,53012
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    campo.value = filtraNumerosComVirgula(campo.value);
    MovimentaCursor(campo, xPos);
}
function formataTelefone(campo, evt) {
    //(00) 0000-0000
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam == 1)
        campo.value = '(' + vr;
    else if (tam >= 2 && tam < 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2);
    else if (tam >= 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2, 5) + '-' + vr.substr(7, 4);
    //(
    //    if(xPos == 1 || xPos == 3 || xPos == 5 || xPos == 9)
    //        xPos = xPos +1
    MovimentaCursor(campo, xPos);
}

function formataTelefonefixo(campo, evt) {
    //(00) 0000-0000
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam == 1)
        campo.value = '(' + vr;
    else if (tam >= 2 && tam < 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2);
    else if (tam >= 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2, 4) + '-' + vr.substr(6, 4);
    //(
    //    if(xPos == 1 || xPos == 3 || xPos == 5 || xPos == 9)
    //        xPos = xPos +1
    MovimentaCursor(campo, xPos);
}

function validaAno(campo) {
    day = data.substring(0, 2);
    month = data.substring(3, 5);
    year = data.substring(6, 10);

    if ((month == 01) || (month == 03) || (month == 05) || (month == 07) || (month == 08) || (month == 10) || (month == 12)) {//mes com 31 dias
        if ((day < 01) || (day > 31)) {
            alert('invalid date');
        }
    } else

        if ((month == 04) || (month == 06) || (month == 09) || (month == 11)) {//mes com 30 dias
            if ((day < 01) || (day > 30)) {
                alert('invalid date');
            }
        } else

            if ((month == 02)) {//February and leap year
                if ((year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0))) {
                    if ((day < 01) || (day > 29)) {
                        alert('invalid date');
                    }
                } else {
                    if ((day < 01) || (day > 28)) {
                        alert('invalid date');
                    }
                }
            }

}

function formataTexto(campo, evt, sMascara) {
    //Nome com Inicial Maiuscula.
    evt = getEvent(evt);
    xPos = PosicaoCursor(campo);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraCaracteres(filtraCampo(campo));
    tam = vr.length;
    if (sMascara == "Aa" || sMascara == "Xx") {
        var valor = campo.value.toLowerCase();
        var count = campo.value.split(" ").length - 1;
        var i;
        var pos = 0;
        var valorIni;
        var valorMei;
        var valorFim;
        valor = valor.substring(0, 1).toUpperCase() + valor.substring(1, valor.length);
        for (i = 0; i < count; i++) {
            pos = valor.indexOf(" ", pos + 1);
            valorIni = valor.substring(0, valor.indexOf(" ", pos - 1)) + " ";
            valorMei = valor.substring(valor.indexOf(" ", pos) + 1, valor.indexOf(" ", pos) + 2).toUpperCase();
            valorFim = valor.substring(valor.indexOf(" ", pos) + 2, valor.length);
            valor = valorIni + valorMei + valorFim;
        }
        campo.value = valor;
    }
    if (sMascara == "Aaa" || sMascara == "Xxx") {
        var valor = campo.value.toLowerCase();
        var count = campo.value.split(" ").length - 1;
        var i;
        var pos = 0;
        var valorIni;
        var valorMei;
        var valorFim;
        var ligacao = false;
        var chrLigacao = Array("de", "da", "do", "para", "e")
        valor = valor.substring(0, 1).toUpperCase() + valor.substring(1, valor.length);
        for (i = 0; i < count; i++) {
            ligacao = false;
            pos = valor.indexOf(" ", pos + 1);
            valorIni = valor.substring(0, valor.indexOf(" ", pos - 1)) + " ";
            for (var a = 0; a < chrLigacao.length; a++) {
                if (valor.substring(valorIni.length, valor.indexOf(" ", valorIni.length)).toLowerCase() == chrLigacao[a].toLowerCase()) {
                    ligacao = true;
                    break;
                }
                else if (ligacao == false && valor.indexOf(" ", valorIni.length) == -1) {
                    if (valor.substring(valorIni.length, valor.length).toLowerCase() == chrLigacao[a].toLowerCase()) {
                        ligacao = true;
                        break;
                    }
                }
            }
            if (ligacao == true) {
                valorMei = valor.substring(valor.indexOf(" ", pos) + 1, valor.indexOf(" ", pos) + 2).toLowerCase();
            }
            else {
                valorMei = valor.substring(valor.indexOf(" ", pos) + 1, valor.indexOf(" ", pos) + 2).toUpperCase();
            }
            valorFim = valor.substring(valor.indexOf(" ", pos) + 2, valor.length);
            valor = valorIni + valorMei + valorFim;
        }
        campo.value = valor;
    }
    MovimentaCursor(campo, xPos);
    return true;
}
// Formata o campo CEP
function formataCEP(campo, evt) {
    //312555-650
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam < 5)
        campo.value = vr;
    else if (tam == 5)
        campo.value = vr + '-';
    else if (tam > 5)
        campo.value = vr.substr(0, 5) + '-' + vr.substr(5);
    MovimentaCursor(campo, xPos);
}

function formataCartaoCredito(campo, evt) {
    //0000.0000.0000.0000
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    var vr = campo.value = filtraNumeros(filtraCampo(campo));
    var tammax = 16;
    var tam = vr.length;
    if (tam < tammax && tecla != 8)
    { tam = vr.length + 1; }
    if (tam < 5)
    { campo.value = vr; }
    if ((tam > 4) && (tam < 9))
    { campo.value = vr.substr(0, 4) + '.' + vr.substr(4, tam - 4); }
    if ((tam > 8) && (tam < 13))
    { campo.value = vr.substr(0, 4) + '.' + vr.substr(4, 4) + '.' + vr.substr(8, tam - 4); }
    if (tam > 12)
    { campo.value = vr.substr(0, 4) + '.' + vr.substr(4, 4) + '.' + vr.substr(8, 4) + '.' + vr.substr(12, tam - 4); }
    MovimentaCursor(campo, xPos);
}

//recupera tecla
//evita criar mascara quando as teclas sÃ£o pressionadas
function teclaValida(tecla) {
    if (tecla == 8 //backspace
        //Esta evitando o post, quando sÃ£o pressionadas estas teclas.
        //Foi comentado pois, se for utilizado o evento texchange, Ã© necessario o post.
        || tecla == 9 //TAB
        || tecla == 27 //ESC
        || tecla == 16 //Shif TAB 
        || tecla == 45 //insert
        || tecla == 46 //delete
        || tecla == 35 //home
        || tecla == 36 //end
        || tecla == 37 //esquerda
        || tecla == 38 //cima
        || tecla == 39 //direita
        || tecla == 40)//baixo
        return false;
    else
        return true;
}
// recupera o evento do form
function getEvent(evt) {
    if (!evt) evt = window.event; //IE
    return evt;
}
//Recupera o cÃ³digo da tecla que foi pressionado
function getKeyCode(evt) {
    var code;
    if (typeof (evt.keyCode) == 'number')
        code = evt.keyCode;
    else if (typeof (evt.which) == 'number')
        code = evt.which;
    else if (typeof (evt.charCode) == 'number')
        code = evt.charCode;
    else
        return 0;
    return code;
}