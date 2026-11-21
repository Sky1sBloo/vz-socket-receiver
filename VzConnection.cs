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

        /// <summary>
        /// Creates a new connection to the server.
        /// </summary>
        /// <param name="ip">Server IP address.</param>
        /// <param name="port">Server port.</param>
        /// <param name="bufferSize">Size of the receive buffer.</param>
        /// <param name="messageDelimiter">Character used to delimit messages (default '|').</param>
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

        /// <summary>
        /// Closes the network stream and TCP client.
        /// </summary>
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
            List<VzType> toReturn = new List<VzType>();
            foreach (string item in splitByTopLevelDelimiter(message))
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

        /// <summary>
        /// Splits a message on the "<<" list delimiter, ignoring delimiters inside
        /// parentheses. Vizzy serializes vectors as "(x<<y<<z)", so those inner
        /// delimiters must not split the vector into separate items.
        /// </summary>
        private static List<string> splitByTopLevelDelimiter(string message)
        {
            List<string> items = new List<string>();
            StringBuilder current = new StringBuilder();
            int parenthesisDepth = 0;

            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];

                if (c == '(')
                {
                    parenthesisDepth++;
                }
                else if (c == ')')
                {
                    parenthesisDepth = Math.Max(0, parenthesisDepth - 1);
                }
                else if (parenthesisDepth == 0 && c == '<' && i + 1 < message.Length && message[i + 1] == '<')
                {
                    items.Add(current.ToString());
                    current.Clear();
                    i++;
                    continue;
                }

                current.Append(c);
            }

            items.Add(current.ToString());
            return items;
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
