using System.Net.Sockets;
using System.Text;

namespace VZ_Sky
{
    /// <summary>
    /// Class representing a connection
    /// Handles sending and receiving of data
    /// </summary>
    public class VzConnection
    {
        private NetworkStream stream;
        private readonly TcpClient client;

        public VzConnection(string ip, int port)
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
        }

        public void Close()
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
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                return null;
            }

            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        private async Task<string?> receiveMessageAsStringAsync()
        {
            byte[] lengthBuffer = new byte[2048];
            int lengthRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
            if (lengthRead == 0)  return null;

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] buffer = new byte[messageLength];
            int totalRead = 0;
            while (totalRead < messageLength) {
                int bytesRead = await stream.ReadAsync(buffer, totalRead, messageLength - totalRead);
                if (bytesRead == 0)  return null;
                totalRead += bytesRead;
            }

            return Encoding.UTF8.GetString(buffer);

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
