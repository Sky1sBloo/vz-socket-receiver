namespace VZ_Sky
{
    /// <summary>
    /// Class representing a Vizzy Program
    /// Use this to automatically handle receiving data
    /// and easily allocate async funtions
    /// </summary>
    public abstract class VzProgram
    {
        /// <summary>
        /// The connection used to send and receive data.
        /// </summary>
        protected VzConnection Connection { get; private set; }

        /// <summary>
        /// Creates a new Vizzy program with the given connection.
        /// </summary>
        /// <param name="connection">The VzConnection instance to use.</param>
        public VzProgram(VzConnection connection)
        {
            this.Connection = connection;
            StartAsyncTask(receiveData);
        }

        /// <summary>
        /// Gets called when program receives a data
        /// </summary>
        public abstract void ReceivedData(List<VzType> values);

        /// <summary>
        /// Create a asynchronous task (generally used onStart functions)
        /// </summary>
        public static void StartAsyncTask(Func<Task> callback)
        {
            Task.Run(callback);
        }

        /// <summary>
        /// Waits until the conditional function is true
        /// </summary>
        ///
        /// <param name="condition">Function condition that ends waiting until this is true</param>
        /// <param name="checkTimeMilliseconds">Time every time this condition is checked</param>
        public static async Task WaitUntil(Func<bool> condition, int checkTimeMilliseconds = 10)
        {
            while (!condition())
            {
                await Task.Delay(checkTimeMilliseconds);
            }
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

}

