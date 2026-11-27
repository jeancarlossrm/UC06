using System.Collections.ObjectModel;
using System.Linq;

using EstoqueMAUI.Models;
using EstoqueMAUI.Dados;

namespace EstoqueMAUI;

public partial class ConsultaProdutosPage : ContentPage
{
    // Guarda o produto selecionado pelo usuário.
    private Produto? produtoSelecionado;

    // Guarda o texto utilizado na pesquisa.
    private string textoPesquisa = "";

    // Lista utilizada para apresentar os produtos na tela.
    private ObservableCollection<Produto> Produtos =>
        ProdutoRepository.Produtos;

    // Construtor da página.
    public ConsultaProdutosPage()
    {
        InitializeComponent();

        // Define a origem dos dados da tela.
        BindingContext = this;

        // Carrega os produtos cadastrados.
        AtualizarLista();
    }

    // =====================================================
    // 1. ATUALIZAR A LISTAGEM
    // Aplica o filtro e atualiza a CollectionView.
    // =====================================================

    private void AtualizarLista()
    {
        var resultado = Produtos
            .Where(p => p.Nome.Contains(
                textoPesquisa,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        ProdutosListView.ItemsSource = resultado;
    }

    // =====================================================
    // 2. PESQUISAR PRODUTOS
    // Executado quando o texto da pesquisa muda.
    // =====================================================

    private void OnPesquisaChanged(
        object sender,
        TextChangedEventArgs e)
    {
        // Guarda o texto digitado.
        textoPesquisa = e.NewTextValue?.Trim() ?? "";

        // Atualiza os resultados da pesquisa.
        AtualizarLista();

        // Limpa a seleção anterior.
        produtoSelecionado = null;

        // Limpa os campos de edição.
        NomeEdicaoEntry.Text = "";
        QuantidadeEdicaoEntry.Text = "";
    }

    // =====================================================
    // 3. SELECIONAR PRODUTO
    // Carrega os dados do produto nos campos de edição.
    // =====================================================

    private void OnProdutoSelecionado(
        object sender,
        SelectionChangedEventArgs e)
    {
        produtoSelecionado = e.CurrentSelection
            .FirstOrDefault() as Produto;

        if (produtoSelecionado == null)
            return;

        // Preenche os campos com os dados selecionados.
        NomeEdicaoEntry.Text = produtoSelecionado.Nome;

        QuantidadeEdicaoEntry.Text =
            produtoSelecionado.Quantidade.ToString();
    }

    // =====================================================
    // 4. ATUALIZAR PRODUTO
    // Valida e salva as alterações.
    // =====================================================

    private async void OnAtualizarClicked(
        object sender,
        EventArgs e)
    {
        // Verifica se existe um produto selecionado.
        if (produtoSelecionado == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione um produto para atualizar.",
                "OK");

            return;
        }

        // Recupera o nome informado.
        string nome = NomeEdicaoEntry.Text?.Trim() ?? "";

        // Valida o nome.
        if (string.IsNullOrWhiteSpace(nome))
        {
            await DisplayAlert(
                "Validação",
                "Informe o nome do produto.",
                "OK");

            NomeEdicaoEntry.Focus();
            return;
        }

        // Valida a quantidade.
        if (!int.TryParse(
            QuantidadeEdicaoEntry.Text,
            out int quantidade) || quantidade <= 0)
        {
            await DisplayAlert(
                "Validação",
                "Informe uma quantidade inteira positiva.",
                "OK");

            QuantidadeEdicaoEntry.Focus();
            return;
        }

        // Atualiza os dados do objeto selecionado.
        produtoSelecionado.Nome = nome;
        produtoSelecionado.Quantidade = quantidade;

        // Limpa a seleção.
        produtoSelecionado = null;
        ProdutosListView.SelectedItem = null;

        // Limpa os campos.
        NomeEdicaoEntry.Text = "";
        QuantidadeEdicaoEntry.Text = "";

        // Atualiza a listagem mantendo a pesquisa.
        AtualizarLista();

        await DisplayAlert(
            "Sucesso",
            "Produto atualizado com sucesso!",
            "OK");
    }

    // =====================================================
    // 5. EXCLUIR PRODUTO
    // Confirma a exclusão antes de remover o registro.
    // =====================================================

    private async void OnExcluirClicked(
        object sender,
        EventArgs e)
    {
        if (produtoSelecionado == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione um produto para excluir.",
                "OK");

            return;
        }

        // Solicita confirmação.
        bool confirmar = await DisplayAlert(
            "Confirmar exclusão",
            $"Deseja excluir o produto {produtoSelecionado.Nome}?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        // Remove o produto da coleção compartilhada.
        Produtos.Remove(produtoSelecionado);

        // Limpa os dados da seleção.
        produtoSelecionado = null;

        ProdutosListView.SelectedItem = null;

        NomeEdicaoEntry.Text = "";
        QuantidadeEdicaoEntry.Text = "";

        // Atualiza a listagem mantendo o filtro.
        AtualizarLista();

        await DisplayAlert(
            "Sucesso",
            "Produto excluído com sucesso!",
            "OK");
    }

    // =====================================================
    // 6. VOLTAR PARA A TELA DE CADASTRO
    // =====================================================

    private async void OnVoltarClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
