using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConsole.Helpers
{
    public class TcpServer
    {
        private readonly TcpListener _listener;
        private readonly X509Certificate2 _certificate;

        public event Func<string, string> OnReceive;
        public event EventHandler<Exception> OnError;
        public event EventHandler OnStarted;
        public event EventHandler OnStopped;
        public event EventHandler<TcpClient> OnClientConnected;
        public event EventHandler<TcpClient> OnClientDisconnected;

        public TcpServer(int port, string certificateFilePath = null, string certificatePassword = null)
        {
            _listener = new TcpListener(System.Net.IPAddress.Any, port);

            if (!string.IsNullOrEmpty(certificateFilePath) && File.Exists(certificateFilePath))
            {
                _certificate = new X509Certificate2(certificateFilePath, certificatePassword);
            }
        }
        private bool running;

        // Define the OnSend event delegate
        public delegate void OnSendEventHandler(object sender, string response);

        // Define the OnSend event
        public event OnSendEventHandler OnSend;

        // ... other methods and events ...

        private void SendResponse(TcpClient client, string response)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(response);
            client.GetStream().Write(bytes, 0, bytes.Length);

            // Raise the OnSend event after sending the response to the client
            OnSend?.Invoke(this, response);
        }

        private void HandleRequest(TcpClient client, string request)
        {
            // Extract the request method and URI from the request
            string[] requestLines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);
            string[] requestLineParts = requestLines[0].Split(' ');
            string method = requestLineParts[0];
            string uri = requestLineParts[1];

            // Raise the OnReceive event, passing in the request string
            string response = OnReceive?.Invoke(request);

            // If the response is null, return a 404 error
            if (response == null)
            {
                response = "HTTP/1.1 404 Not Found\r\n\r\n";
            }
            else
            {
                // If the response is a JSON string, add a Content-Disposition header to download it as a file
                if (response.StartsWith("{") || response.StartsWith("["))
                {
                    response = $"HTTP/1.1 200 OK\r\nContent-Disposition: attachment; filename=data.json\r\nContent-Type: application/json\r\n\r\n{response}";
                }
                else
                {
                    response = $"HTTP/1.1 200 OK\r\n\r\n{response}";
                }
            }

            // Convert the response string to a byte array and send it to the client
            byte[] buffer = Encoding.UTF8.GetBytes(response);
            client.GetStream().Write(buffer, 0, buffer.Length);

            // Raise the OnSend event, passing in the response string
            OnSend?.Invoke(this, response);
        }


        public void Start()
        {
            _listener.Start();
            OnStarted?.Invoke(this, EventArgs.Empty);
            AcceptClients();
        }

        public void Stop()
        {
            _listener.Stop();
            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        private async void AcceptClients()
        {
            while (true)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();

                    OnClientConnected?.Invoke(this, client);

                    Task.Run(() => ProcessClient(client));
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, ex);
                }
            }
        }

        private async Task ProcessClient(TcpClient client)
        {
            Stream stream = client.GetStream();

            if (_certificate != null)
            {
                SslStream sslStream = new SslStream(stream);
                try
                {
                    await sslStream.AuthenticateAsServerAsync(_certificate, false, SslProtocols.Tls12, true);
                    stream = sslStream;
                }
                catch (AuthenticationException ex)
                {
                    OnError?.Invoke(this, ex);
                    client.Close();
                    return;
                }
            }

            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Extract the parameters from the request
            var lines = request.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var firstLine = lines.FirstOrDefault();
            var parts = firstLine?.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var method = parts?.Length > 0 ? parts[0] : null;
            var path = parts?.Length > 1 ? parts[1] : null;
            var version = parts?.Length > 2 ? parts[2] : null;

            var parameters = new Dictionary<string, string>();
            foreach (var line in lines.Skip(1))
            {
                var parts2 = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts2.Length == 2)
                {
                    parameters[parts2[0]] = parts2[1];
                }
            }

            string response = OnReceive?.Invoke(request);

            if (!string.IsNullOrEmpty(response))
            {
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }

            client.Close();
            OnClientDisconnected?.Invoke(this, client);
        }

    }
}
