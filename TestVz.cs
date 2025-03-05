using VZ_Sky;

public class TestProgram : VzProgram
{
    public TestProgram(VzConnection connection) : base(connection)
    {
    }

    public override async Task OnStart()
    {
        while (true)
        {
            List<VzType> toReturn = new List<VzType> { new VzType(5), new VzType(true), new VzType("test") };
            await Connection.SendDataAsync(toReturn);
            await Task.Delay(1000);
        }
    }

    public override void ReceivedData(List<VzType> values)
    {
        Console.Write("Received: ");
        foreach (VzType value in values)
        {
            Console.Write(value.ToString());
            Console.Write(", ");
        }
        Console.WriteLine();
    }
}

