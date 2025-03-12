# vz-socket-receiver 
Library for receiving and sending data on Juno New Origins through [Sockets service for Vizzy](https://www.simplerockets.com/Mods/View/298478/Sockets-service-for-Vizzy)

## Features
 - Sending and Receiving data through Sockets Service Mod
 - Simplifies asynchronous function handling
 - Data is statically typed on float, bool, string, and vector

## Usage
All classes in this library are in the `VZ_Sky` namespace
The library provides 3 classes:
1. VzConnection
2. VzProgram
3. VzType

Both the `VzProgram` and `VzConnection` is used to handle the program logic and connection

### VzConnection 
Sets up connection to the server. Handles sending and receiving data both synchronously and asynchronously.
Documentation is included in the `VzConnection.cs` file.

### VzProgram
Handles the program logic. Receiving data and sending is handled through this class. See `VzProgram.cs`.
> Note: Call the base constructor

### VzType
Handles the Vizzy Variable data types for static typing

## Example Usage
**Program.cs**
```
using VZ_Sky;

string SERVER_IP = "127.0.0.1";
int SERVER_PORT = 10809;
VzConnection connection = new VzConnection(SERVER_IP, SERVER_PORT);
TestProgram testProgram = new TestProgram(connection);

await Task.Delay(-1);
```
**TestProgram.cs**
using VZ_Sky;

```
public class TestProgram : VzProgram
{
    public TestProgram(VzConnection connection) : base(connection)
    {
        StartAsyncTask(OnStart);
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
```
