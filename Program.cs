using VZ_Socket;

string SERVER_IP = "127.0.0.1";
int SERVER_PORT = 10809;
VzConnection connection = new VzConnection(SERVER_IP, SERVER_PORT);
TestProgram testProgram = new TestProgram(connection);
testProgram.Run();
