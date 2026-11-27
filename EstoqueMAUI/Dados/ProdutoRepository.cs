using System.Collections.ObjectModel;
using EstoqueMAUI.Models;

namespace EstoqueMAUI.Dados;

public static class ProdutoRepository
{
    public static ObservableCollection<Produto> Produtos { get; } = new();
}