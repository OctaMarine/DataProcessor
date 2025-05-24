namespace DataProcessor;

public class Program
{
    public static void Main(string[] args)
    {
        RunCompression();
        
        RunParallelServer();

        RunStandardizeLog();
    }

    private static void RunCompression()
    {
        var compression = new Compression();

        var strForCompression = "aaabbcccdde";
        var compressionResult = compression.Compress(strForCompression);
        Console.WriteLine(compressionResult);

        var decompressionResult = compression.Decompress(compressionResult);
        Console.WriteLine(decompressionResult);
    }

    private static void RunParallelServer()
    {
        var reader1 = new Reader();
        var reader2 = new Reader();
        var reader3 = new Reader();
        var reader4 = new Reader();
        var writer1 = new Writer();
        var writer2 = new Writer();
        var writer3 = new Writer();

        var threadRead1 = new Thread(reader1.Read);
        threadRead1.Name = "Reader-1";
        var threadRead2 = new Thread(reader2.Read);
        threadRead2.Name = "Reader-2";
        var threadRead3 = new Thread(reader3.Read);
        threadRead3.Name = "Reader-3";
        var threadRead4 = new Thread(reader4.Read);
        threadRead4.Name = "Reader-4";

        var threadWrite1 = new Thread(writer1.Write);
        threadWrite1.Name = "Writer-1";
        var threadWrite2 = new Thread(writer2.Write);
        threadWrite2.Name = "Writer-2";
        var threadWrite3 = new Thread(writer3.Write);
        threadWrite3.Name = "Writer-3";
        
        threadWrite1.Start();
        threadRead1.Start();
        threadRead2.Start();
        threadRead3.Start();
        threadWrite2.Start();
        threadWrite3.Start();
        threadRead4.Start();
    }

    private static void RunStandardizeLog()
    {
        var standardizeLog = new StandardizeLog();
        standardizeLog.Standardize(); 
    }
}