namespace DataProcessor;

public static class ParallelServer
{
    private static int _count;
    private static readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

    public static int GetCount()
    {
        _rwLock.EnterReadLock();
        try
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} start");

            Thread.Sleep(100);
            return _count;
        }
        finally
        {
            Console.WriteLine($"{Thread.CurrentThread.Name}: Read {_count}");
            _rwLock.ExitReadLock();

        }
    }

    public static void AddCount(int value)
    {
        _rwLock.EnterWriteLock();
        try
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} start");
            Thread.Sleep(300);
            _count += value;
        }
        finally
        {
            Console.WriteLine($"{Thread.CurrentThread.Name}: Added {value}");
            _rwLock.ExitWriteLock();
        }
    }
}

public class Writer
{
    public void Write()
    {
        ParallelServer.AddCount(5);
    }
}

public class Reader
{
    public void Read()
    {
        var count = ParallelServer.GetCount();
    }
}
