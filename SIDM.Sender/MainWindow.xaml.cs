using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace SIDM.Sender;

public partial class MainWindow : Window
{
    private HubConnection _connection;

    public MainWindow()
    {
        InitializeComponent();

        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5271/sidmHub")
            .WithAutomaticReconnect()
            .Build();

        // Llamamos a un método dedicado para conectar
        ConectarServidor();
    }

    private async void ConectarServidor()
    {
        try
        {
            // Esperamos a que la conexión se complete realmente
            await _connection.StartAsync();
            this.Title = "SIDM - Conectado";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error de conexión: " + ex.Message);
        }
    }

    private async void Send_Click(object sender, RoutedEventArgs e)
    {
        // Validación de seguridad: Si no está conectado, reintenta
        if (_connection.State != HubConnectionState.Connected)
        {
            MessageBox.Show("Aún no hay conexión con el servidor. Reintentando...");
            try { await _connection.StartAsync(); } catch { return; }
        }

        if (string.IsNullOrWhiteSpace(txtInput.Text))
        {
            MessageBox.Show("Por favor, escribe un mensaje.");
            return;
        }

        string mensaje = txtInput.Text;
        string nivel = (cbNivel.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();

        try
        {
            await _connection.InvokeAsync("SendAlert", mensaje, nivel);
            txtInput.Clear();
            MessageBox.Show("Mensaje difundido correctamente.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fallo en el envío: {ex.Message}");
        }
    }
}