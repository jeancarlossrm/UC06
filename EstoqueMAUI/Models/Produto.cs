namespace EstoqueMAUI.Models;

public class Produto
{
    public string Nome { get; set; }
    public int Quantidade { get; set; }

    public Produto(string nome, int quantidade)
    {
        Nome = nome;
        Quantidade = quantidade;
    }
}