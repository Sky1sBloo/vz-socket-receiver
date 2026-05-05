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
        private readonly StringBuilder receiveBuffer = new StringBuilder();
        private readonly int bufferSize;
        private char messageDelimiter;

        public VzConnection(string ip, int port, int bufferSize, char messageDelimiter = '|')
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            this.bufferSize = bufferSize;
            this.messageDelimiter = messageDelimiter;
        }

        /// <summary>
        /// Determins if client is still connected
        /// </summary>
        public bool IsConnected()
        {
            return client.Connected;
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
        /// Method for sending data asynchrounously
        /// </summary>
        public async Task SendDataAsync(List<VzType> list)
        {

            String message = parseVzTypeToString(list);
            await sendMessageAsBytesAsync(message);
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
                    VzType type = new VzType();
                    type.InferTypeFromString(item);
                    toReturn.Add(type);
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
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Dispose();
                    return null;
                }
                receiveBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                string tmp = receiveBuffer.ToString();
                int idx = tmp.IndexOf(messageDelimiter);
                if (idx >= 0)
                {
                    string result = tmp.Substring(0, idx);
                    receiveBuffer.Remove(0, idx + 1);
                    return result;
                }
            }
        }

        private async Task<string?> receiveMessageAsStringAsync()
        {
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Dispose();
                    return null;
                }
                receiveBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                string tmp = receiveBuffer.ToString();
                int idx = tmp.IndexOf(messageDelimiter);
                if (idx >= 0)
                {
                    string result = tmp.Substring(0, idx);
                    receiveBuffer.Remove(0, idx + 1);
                    return result;
                }
            }
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
