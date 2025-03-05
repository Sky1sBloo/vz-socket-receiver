using System.Net.Sockets;
using System.Text;

namespace VZ_Socket
{
    public class VzConnection
    {

        private NetworkStream stream;

        public VzConnection(TcpClient client)
        {
            stream = client.GetStream();
        }

        public void SendData(List<VzType> list) {
            String message = parseVzTypeToString(list);
            sendMessageAsBytes(message);
        }

        public List<VzType>? ReceiveData() {
            string? message = receiveMessageAsString();
            if (message == null) {
                return null;
            }
            return parseMessageToList(message);
        }

        private static List<VzType> parseMessageToList(string message)
        {
            string[] items = message.Split("<<");
            List<VzType> toReturn = new List<VzType>();
            foreach (string item in items) {
                if (!string.IsNullOrWhiteSpace(item)) {
                    toReturn.Add(new VzType(message));
                }
            }

            return toReturn;
        }

        private static string parseVzTypeToString(List<VzType> data) {
            string toReturn = "";
            foreach (VzType item in data) {
                toReturn += item.ToString();
                toReturn += "<<";
            }
            return toReturn;
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

        private void sendMessageAsBytes(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(messageBytes);
        }

    }
}
