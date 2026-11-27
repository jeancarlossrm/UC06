using EstoqueMAUI.Models;
using EstoqueMAUI.Dados;

namespace EstoqueMAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnAdicionarClicked(object sender, EventArgs e)
    {
        string nome = NomeEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(nome))
        {
            ResultadoLabel.Text = "Informe o nome do produto.";
            NomeEntry.Focus();
            return;
        }

        if (!int.TryParse(QuantidadeEntry.Text, out int quantidade) || quantidade <= 0)
        {
            ResultadoLabel.Text = "Informe uma quantidade inteira positiva.";
            QuantidadeEntry.Focus();
            return;
        }

        Produto novoProduto = new Produto(nome, quantidade);
        ProdutoRepository.Produtos.Add(novoProduto);

        ResultadoLabel.Text = $"Produto cadastrado: {nome} | Quantidade: {quantidade}";

        NomeEntry.Text = "";
        QuantidadeEntry.Text = "";
        NomeEntry.Focus();
    }

    // Abre a tela de consulta
    private async void OnConsultarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ConsultaProdutosPage));
    }
}