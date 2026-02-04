using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace SIDM.Receiver;

public partial class MainWindow : Window
{
    // Aquí declaramos la variable para que TODO el archivo la reconozca
    private HubConnection _connection;

    public MainWindow()
    {
        InitializeComponent();

        // Configuramos la conexión
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5271/sidmHub")
            .WithAutomaticReconnect()
            .Build();

        // Escuchamos la alerta
        _connection.On<string, string>("ReceiveAlert", (message, level) =>
        {
            Dispatcher.Invoke(() => {
                txtMensaje.Text = message;

                if (level == "Emergencia")
                {
                    MainBorder.Background = System.Windows.Media.Brushes.DarkRed;
                    txtTitulo.Foreground = System.Windows.Media.Brushes.White;
                    txtMensaje.Foreground = System.Windows.Media.Brushes.White;
                }
                else
                {
                    MainBorder.Background = System.Windows.Media.Brushes.White;
                    txtTitulo.Foreground = System.Windows.Media.Brushes.Red;
                    txtMensaje.Foreground = System.Windows.Media.Brushes.Black;
                }

                this.Show();
                this.Topmost = true;
            });
        });

        // Iniciamos la conexión sin bloquear la interfaz
        Task.Run(async () => {
            try { await _connection.StartAsync(); } catch { }
        });
    }

    // Este es el método que el XAML no encontraba
    private async void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        if (_connection != null && _connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("Ack");
        }
        this.Hide();
    }
}