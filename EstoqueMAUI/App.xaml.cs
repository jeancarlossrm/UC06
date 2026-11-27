namespace EstoqueMAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Cria a janela principal da aplicação.
            var window = new Window(new AppShell());
            // Define a largura inicial da janela.
            window.Width = 1000;
            // Define a altura inicial da janela.
            window.Height = 650;
            // Define a largura mínima permitida.
            window.MinimumWidth = 1000;
            // Define a largura máxima permitida.
            window.MaximumWidth = 1000;
            // Define a altura mínima permitida.
            window.MinimumHeight = 650;
            // Define a altura máxima permitida.
            window.MaximumHeight = 650;
            // Retorna a janela configurada.
            return window;
        }

    }
}