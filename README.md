# EstoqueMAUI — Guia de Estudo 

Aplicativo desktop/multiplataforma em **.NET MAUI** para **controle de estoque de produtos**, desenvolvido seguindo as aulas da pasta `Aula`. O projeto permite **cadastrar, consultar, pesquisar, editar e excluir** produtos armazenados em memória.

---

## 1. Estrutura do Projeto

```text
EstoqueMAUI/
├── EstoqueMAUI.sln
└── EstoqueMAUI/
    ├── App.xaml / App.xaml.cs              # Inicialização da aplicação e tamanho da janela (1000x650)
    ├── AppShell.xaml / AppShell.xaml.cs    # Estrutura de navegação Shell e registro de rotas
    ├── MainPage.xaml / MainPage.xaml.cs    # Tela 1: Cadastro de produtos
    ├── ConsultaProdutosPage.xaml / .cs     # Tela 2: Consulta, pesquisa, edição e exclusão
    ├── Models/
    │   └── Produto.cs                      # Modelo de dados (Nome e Quantidade)
    └── Dados/
        └── ProdutoRepository.cs            # Repositório estático compartilhado em memória
```

---

## 2. Configuração da Janela (`App.xaml.cs`)

No método `CreateWindow`, a aplicação inicia usando o `AppShell` e fixa as dimensões da janela no Windows em **1000 × 650** unidades:

- `Width` / `Height`: largura e altura iniciais da janela.
- `MinimumWidth` / `MaximumWidth`: trava a largura mínima e máxima em `1000`.
- `MinimumHeight` / `MaximumHeight`: trava a altura mínima e máxima em `650`.

> **Conceito:** Dimensão da **janela** é o tamanho externo do programa no sistema operacional; dimensão dos **componentes** (`WidthRequest`, `HeightRequest`) controla o tamanho interno de botões, campos e painéis.

---

## 3. Modelo de Dados e Repositório Compartilhado

### `Models/Produto.cs`
Define a estrutura de cada produto com duas propriedades e um construtor:

```csharp
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
```

### `Dados/ProdutoRepository.cs`
Guarda a lista de produtos de forma compartilhada entre todas as telas:

```csharp
public static class ProdutoRepository
{
    public static ObservableCollection<Produto> Produtos { get; } = new();
}
```

- **`static class`**: permite acessar `ProdutoRepository.Produtos` de qualquer tela sem precisar dar `new`.
- **`ObservableCollection<Produto>`**: coleção dinâmica do .NET que notifica a interface quando itens são adicionados ou removidos.

---

## 4. Navegação entre Telas (MAUI Shell)

1. **Registro da rota (`AppShell.xaml.cs`):**
   ```csharp
   Routing.RegisterRoute(
       nameof(ConsultaProdutosPage),
       typeof(ConsultaProdutosPage));
   ```
   Sem esse registro, o Shell não sabe localizar a segunda página pelo nome.

2. **Avançar para a tela de consulta (`MainPage.xaml.cs`):**
   ```csharp
   await Shell.Current.GoToAsync(nameof(ConsultaProdutosPage));
   ```

3. **Voltar para a tela anterior (`ConsultaProdutosPage.xaml.cs`):**
   ```csharp
   await Shell.Current.GoToAsync("..");
   ```

> **Atenção:** Os métodos de navegação usam `async` e `await` porque a transição de página é uma operação assíncrona.

---

## 5. Tela 1 — Cadastro e Design (`MainPage`)

### Organização Visual (`MainPage.xaml`)
- **Estilos reutilizáveis (`ContentPage.Resources`):**
  - `CampoEstilo` (`TargetType="Entry"`): padroniza altura (`45`), fonte (`14`), cores de texto/placeholder e fundo `#F1F5F9`.
  - `BotaoEstilo` (`TargetType="Button"`): padroniza altura (`45`), largura (`220`), `CornerRadius="9"`, fonte em negrito e cor `#005CA9`.
  - Aplicados via `Style="{StaticResource NomeDoEstilo}"`.
- **Layout com `Grid` principal (`RowDefinitions="68,*"`, `ColumnDefinitions="210,*"`):**
  - **Linha 0 (`Grid.ColumnSpan="2"`):** Cabeçalho superior (`#0B2D4D`) com título `ESTOQUE MAUI`.
  - **Linha 1, Coluna 0:** Menu lateral (`#123B5D`) com itens, divisor (`BoxView` com `Opacity="0.25"`) e rodapé.
  - **Linha 1, Coluna 1:** `ScrollView` com fundo `#F1F5F9`, cartão branco (`Border` com `RoundRectangle 14`) para o formulário e painel informativo com transparência.
- **Dimensionamento no `Grid`:**
  - Valor fixo (ex.: `68`, `210`): tamanho absoluto.
  - `Auto`: ajusta ao tamanho do conteúdo.
  - `*`: ocupa todo o espaço restante disponível.
- **Cores com transparência (Canal Alpha vs `Opacity`):**
  - `#33005CA9` aplica transparência apenas na cor (os 2 primeiros dígitos `33` são o canal Alpha), mantendo o texto interno nítido.
  - `Opacity="0.25"` aplica transparência ao componente inteiro e seus filhos.

### Lógica de Cadastro (`MainPage.xaml.cs`)
No evento `OnAdicionarClicked`:
1. Captura e limpa espaços do nome: `string nome = NomeEntry.Text?.Trim() ?? "";`.
2. Valida se o nome está vazio (`string.IsNullOrWhiteSpace(nome)`), exibe aviso no `ResultadoLabel` e devolve o foco com `NomeEntry.Focus()`.
3. Valida se a quantidade é um inteiro positivo (`!int.TryParse(QuantidadeEntry.Text, out int quantidade) || quantidade <= 0`).
4. Instancia `new Produto(nome, quantidade)` e adiciona em `ProdutoRepository.Produtos.Add(novoProduto)`.
5. Atualiza `ResultadoLabel`, limpa `NomeEntry` e `QuantidadeEntry` e retorna o foco para `NomeEntry`.

---

## 6. Tela 2 — Consulta, Pesquisa, Edição e Exclusão (`ConsultaProdutosPage`)

### Interface (`ConsultaProdutosPage.xaml`)
Dividida em um `Grid` de 4 linhas (`RowDefinitions="Auto,Auto,*,Auto"`):
1. **Linha 0:** Cabeçalho (`GESTÃO DE PRODUTOS`).
2. **Linha 1:** Campo de busca (`PesquisaEntry`) ligado ao evento `TextChanged="OnPesquisaChanged"`.
3. **Linha 2 (`*`):** `CollectionView` (`ProdutosListView`) com:
   - `SelectionMode="Single"` e `SelectionChanged="OnProdutoSelecionado"`.
   - `EmptyView`: exibe `"Nenhum produto encontrado."` quando a lista filtrada está vazia.
   - `ItemTemplate` / `DataTemplate`: renderiza cada `Produto` em um `Border` com `{Binding Nome}` e `{Binding Quantidade, StringFormat='Quantidade: {0}'}`.
4. **Linha 3:** Cartão de edição com `NomeEdicaoEntry`, `QuantidadeEdicaoEntry` (`Keyboard="Numeric"`), botões **Salvar alterações** (`#005CA9`), **Excluir produto** (`#DC2626`) e **Voltar ao cadastro** (`#64748B`).

### Lógica (`ConsultaProdutosPage.xaml.cs`)
- **`BindingContext = this;`**: conecta os bindings do XAML às propriedades da própria página.
- **`AtualizarLista()`**: filtra `ProdutoRepository.Produtos` com LINQ usando `StringComparison.OrdinalIgnoreCase` (ignora maiúsculas/minúsculas) e atualiza `ProdutosListView.ItemsSource`.
- **`OnPesquisaChanged`**: atualiza `textoPesquisa`, chama `AtualizarLista()` e limpa seleção/campos de edição.
- **`OnProdutoSelecionado`**: obtém `e.CurrentSelection.FirstOrDefault() as Produto` e preenche `NomeEdicaoEntry` e `QuantidadeEdicaoEntry`.
- **`OnAtualizarClicked`**: valida se há item selecionado, valida nome e quantidade positiva, atualiza `produtoSelecionado.Nome` e `produtoSelecionado.Quantidade`, limpa seleção/campos, chama `AtualizarLista()` e exibe `DisplayAlert` de sucesso.
- **`OnExcluirClicked`**: exige seleção, pede confirmação com `await DisplayAlert("Confirmar exclusão", ..., "Sim", "Não")`, remove o item de `Produtos`, limpa a tela e atualiza a listagem.

---

## 7. Glossário de Conceitos

| Termo | O que faz |
| :--- | :--- |
| `ContentPage` | Representa uma tela da aplicação. |
| `Grid` | Organiza elementos em linhas (`RowDefinitions`) e colunas (`ColumnDefinitions`). |
| `Border` | Cria cartões e contêineres com bordas e cantos arredondados (`StrokeShape`). |
| `Style` / `StaticResource` | Reutiliza configurações visuais entre vários componentes. |
| `BindingContext` | Define a fonte de dados usada pelos `{Binding ...}` da página. |
| `CollectionView` | Exibe listas de itens com suporte a seleção e templates. |
| `DataTemplate` | Define o visual de cada item dentro da `CollectionView`. |
| `EmptyView` | Conteúdo exibido quando a lista está vazia. |
| `ObservableCollection<T>` | Lista que notifica alterações de adição/remoção. |
| `DisplayAlert` | Exibe pop-ups assíncronos de aviso, validação ou confirmação. |
| `GoToAsync` | Realiza a navegação entre páginas no MAUI Shell. |

---

## 8. Como Compilar via Terminal

Na pasta da solução (`EstoqueMAUI`), execute:

```powershell
dotnet build EstoqueMAUI.sln -f net9.0-windows10.0.19041.0
```
