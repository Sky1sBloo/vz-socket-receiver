using System.Net.Sockets;
using System.Text;

namespace VZ_Sky
{
    /// <summary>
    /// Class representing a connection
    /// Handles sending and receiving of data
    /// </summary>
    public class VzConnection : IDisposable
    {
        private NetworkStream stream;
        private readonly TcpClient client;

        public VzConnection(string ip, int port)
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
        }

        public void Dispose()
        {
            stream?.Close();
            client?.Close();
        }

        /// <summary>
        /// Method for sending data 
        /// </summary>
        public void SendData(List<VzType> list)
        {
            String message = parseVzTypeToString(list);
            sendMessageAsBytes(message);
        }

        /// <summary>
        /// Method for receiving data
        /// Blocks the function until it receives
        /// </summary>
        public List<VzType> ReceiveData()
        {
            string? message = receiveMessageAsString();
            if (message == null)
            {
                return new List<VzType>();
            }
            return parseMessageToList(message);
        }

        /// <summary>
        /// Method for receiving asynchrounously 
        /// </summary>
        public async Task<List<VzType>> ReceiveDataAsync()
        {
            string? message = await receiveMessageAsStringAsync();
            if (message == null)
            {
                return new List<VzType>();
            }
            return parseMessageToList(message);
        }

        private static List<VzType> parseMessageToList(string message)
        {
            string[] items = message.Split("<<");
            List<VzType> toReturn = new List<VzType>();
            foreach (string item in items)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    toReturn.Add(new VzType(item));
                }
            }

            return toReturn;
        }

        private static string parseVzTypeToString(List<VzType> data)
        {
            var toReturn = new StringBuilder();
            foreach (VzType item in data)
            {
                toReturn.Append(item.ToString());
                toReturn.Append("<<");
            }
            return toReturn.Length > 2 ? toReturn.ToString(0, toReturn.Length - 2) : toReturn.ToString();
        }

        private string? receiveMessageAsString()
        {
            byte[] buffer = new byte[2048];
            int totalBytesRead = 0;
            while (totalBytesRead < buffer.Length)
            {
                int bytesRead = stream.Read(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                if (bytesRead == 0) break; // Connection closed
                totalBytesRead += bytesRead;
            }
            return Encoding.UTF8.GetString(buffer, 0, totalBytesRead);

        }

        private async Task<string?> receiveMessageAsStringAsync()
        {
            byte[] buffer = new byte[2048];
            int totalBytesRead = 0;
            while (totalBytesRead < buffer.Length)
            {
                int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                if (bytesRead == 0) break; // Connection closed
                totalBytesRead += bytesRead;
            }
            return Encoding.UTF8.GetString(buffer, 0, totalBytesRead);
        }

        private void sendMessageAsBytes(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(messageBytes);
        }

        private async Task sendMessageAsBytesAsync(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            await stream.FlushAsync();
        }
    }
}
