﻿using VZ_Socket;
using System.Net.Sockets;

string SERVER_IP = "127.0.0.1";
int SERVER_PORT = 10809;
using TcpClient tcpClient = new TcpClient(SERVER_IP, SERVER_PORT);
VzConnection connection = new VzConnection(tcpClient);

while (true)
{
}
