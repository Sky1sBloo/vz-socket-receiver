
using VZ_Socket;
public class TestProgram
{
    private VzConnection connection;
    public TestProgram(VzConnection connection)
    {
        this.connection = connection;
    }

    public void Run()
    {
        Task.Run(printMessages);
        while (true)
        {
            List<VzType> toReturn = new List<VzType> { new VzType(5), new VzType(true), new VzType("test") };

            Thread.Sleep(1000);
            connection.SendData(toReturn);
        }
    }

    private async void printMessages()
    {
        while (true)
        {
            List<VzType> values = await connection.ReceiveDataAsync();
            foreach (VzType value in values)
            {
                Console.WriteLine(value.ToString());
            }

        }
    }
}

