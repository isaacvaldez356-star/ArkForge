using System.Net.Sockets;
using System.Text;

namespace ArkForge.Rcon
{
    public class RconClient : IDisposable
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private int _requestId = 0;

        public bool IsConnected => _client?.Connected ?? false;

        public async Task<bool> ConnectAsync(string host, int port, string password)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(host, port);
                _stream = _client.GetStream();

                var authResponse = await SendPacketAsync(3, password); // 3 = SERVERDATA_AUTH
                return authResponse != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> SendCommandAsync(string command)
        {
            if (!IsConnected)
                return null;

            return await SendPacketAsync(2, command); // 2 = SERVERDATA_EXECCOMMAND
        }

        private async Task<string?> SendPacketAsync(int type, string body)
        {
            if (_stream == null)
                return null;

            _requestId++;
            var bodyBytes = Encoding.UTF8.GetBytes(body);
            var packetSize = 4 + 4 + bodyBytes.Length + 2;

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write(packetSize);
            writer.Write(_requestId);
            writer.Write(type);
            writer.Write(bodyBytes);
            writer.Write((byte)0);
            writer.Write((byte)0);

            var packet = ms.ToArray();
            await _stream.WriteAsync(packet);

            var responseBuffer = new byte[4096];
            var bytesRead = await _stream.ReadAsync(responseBuffer);

            if (bytesRead < 12)
                return null;

            var responseId = BitConverter.ToInt32(responseBuffer, 4);
            if (responseId == -1)
                return null; // Autenticación fallida

            var responseBodyLength = bytesRead - 12;
            return Encoding.UTF8.GetString(responseBuffer, 12, responseBodyLength).TrimEnd('\0');
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Dispose();
        }
    }
}