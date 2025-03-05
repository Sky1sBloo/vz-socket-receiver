using VZ_Socket;

string SERVER_IP = "127.0.0.1";
int SERVER_PORT = 10809;
VzConnection connection = new VzConnection(SERVER_IP, SERVER_PORT);

while (true)
{
    List<VzType> receive = connection.ReceiveData();
    List<VzType> toReturn = new List<VzType>{
        new VzType(5),
        new VzType(true),
        new VzType("test") };

    connection.SendData(toReturn);
}
