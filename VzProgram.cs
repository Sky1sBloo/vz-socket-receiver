using VZ_Sky;


/// <summary>
/// Class representing a Vizzy Program
/// Use this to automatically handle receiving data
/// and easily allocate async funtions
/// </summary>
public abstract class VzProgram
{
    // Call this if you need to send or receive data
    protected VzConnection Connection { get; private set; }

    public VzProgram(VzConnection connection)
    {
        this.Connection = connection;
        startAsyncTask(receiveData);
        startAsyncTask(OnStart);
    }


    /// <summary>
    /// Automatically called on start of program
    /// Runs on a separate thread to allow for delay
    /// (Ensure that it is async function)
    /// </summary>
    public abstract Task OnStart();

    /// <summary>
    /// Gets called when program receives a data
    /// </summary>
    public abstract void ReceivedData(List<VzType> values);

    /// <summary>
    /// Create a asynchronous task (generally used onStart functions)
    /// </summary>
    public void startAsyncTask(Func<Task> callback)
    {
        Task.Run(callback);
    }

    /// <summary>
    /// Function call to call ReceivedData when it receives a message
    /// </summary>
    private async Task receiveData()
    {
        while (true)
        {
            List<VzType> values = await Connection.ReceiveDataAsync();
            ReceivedData(values);
        }
    }
}

