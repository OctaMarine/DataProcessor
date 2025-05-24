using System.Text;

namespace DataProcessor;

public class StandardizeLog
{
    private const string LOG_DIR = "Data/Logs";
    private const string OUT_DIR = "Data/Out";
    private const string PROBLEM_DIR = "Data/Problem";
    
    private const string DEFAULT_FUNC = "DEFAULT";
    private const string PROGRAM_VERSION_PATTERN = "Версия программы:";
    private const char SEPARATOR = '\t';

    private enum LogLevel
    {
        INFO,
        WARN,
        ERROR,
        DEBUG
    }

    private enum OldLogLevel
    {
        INFORMATION,
        WARNING
    }

    public void Standardize()
    {
        //check directory
        if (!Directory.Exists(LOG_DIR))
        {
            Console.WriteLine("Directory not found");
        }
        var files = Directory.GetFiles(LOG_DIR);

        if (files.Length == 0)
        {
            Console.WriteLine("Directory is empty");
        }

        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"Parse file {files[0]} ...");
            var result = StandardizeFile(files[i]);
            if (result)
            {
                Console.WriteLine($"Success standardize file: {files[i]}");
            }
            else
            {
                Console.WriteLine($"Failed standardize file: {files[i]}");
                if (!Directory.Exists(PROBLEM_DIR))
                {
                    Directory.CreateDirectory(PROBLEM_DIR);
                }

                File.Copy(files[i], $"{PROBLEM_DIR}/problems_{Path.GetFileName(files[i])}", overwrite: true);
                Console.WriteLine($"Copy file: {files[i]} to Problem dir");

            }
        }
    }

    private bool StandardizeFile(string file)
    {
        var text = File.ReadAllText(file);
        if (text.Length <= 10)
        {
            Console.WriteLine("Not valid");
            return false;
        }
        
        //try format 1
        var dateSeparatorTimeIndex = text.IndexOf(' ');
        if (dateSeparatorTimeIndex == -1 && text.Length<= dateSeparatorTimeIndex+1)
        {
            Console.WriteLine("Not valid format");
            return false;
        }
        var dateSeparatorIndex = text.IndexOf(' ',  dateSeparatorTimeIndex+1);

        DateTime data;
        var builder = new StringBuilder();
        //check parse
        var isParsed = DateTime.TryParse(text.Substring(0,dateSeparatorIndex), out data);
        if (isParsed)
        {
            var result = ParseFirstFormat(text, builder);
            if (!result)
            {
                return false;
            }
            return SaveToFile(Path.GetFileName(file),builder.ToString());
        }
        else 
        {
            //try format 2
            dateSeparatorIndex = text.IndexOf('|');
            if (dateSeparatorIndex == -1)
            {
                Console.WriteLine("Not valid format");
                return false;
            }
            //check parse
            isParsed = DateTime.TryParse(text.Substring(0,dateSeparatorIndex), out data);
            if (isParsed)
            {
                var result = ParseSecondFormat(text, builder);
                if (!result)
                {
                    return false;
                }
                return SaveToFile(Path.GetFileName(file),builder.ToString());
            }
            else
            {
                Console.WriteLine("Not valid format");
                return false;
            }
                
        }
        return true;
    }

    private bool ParseFirstFormat(string text,StringBuilder builder)
    {
        var parseSeparator = ' ';
        
        var parts = text.Split(parseSeparator);
        if (parts.Length != 6)
        {
            Console.WriteLine("Not valid format");
            return false;
        }
        
        if(!ParseData(parts[0] +" "+ parts[1], builder))
        {
            return false;
        }
        builder.Append(SEPARATOR);
        if(!ParseLogLevel(parts[2], builder))
        {
            return false;
        }
        builder.Append(SEPARATOR);
        if(!ParseCallFunc(String.Empty, builder))
        {
            return false;
        }
        builder.Append(SEPARATOR);
        if(!ParseProgramVersion($"{parts[3]} {parts[4]}",parts[5], builder))
        {
            return false;
        }
        
        return true;
    }
    
    private bool ParseSecondFormat(string text,StringBuilder builder)
    {
        var parseSeparator = '|';
        
        var parts = text.Split(parseSeparator);
        if (parts.Length != 5)
        {
            Console.WriteLine("Not valid format");
            return false;
        }
        
        if(!ParseData(parts[0], builder))
        {
            return false;
        }
        builder.Append(SEPARATOR);
        if(!ParseLogLevel(parts[1], builder))
        {
            return false;
        }
        builder.Append(SEPARATOR);
        if(!ParseCallFunc(parts[3], builder))
        {
            return false;
        }
        builder.Append(SEPARATOR);
        builder.Append(parts[4]);
        
        return true;
    }

    private bool ParseData(string srt, StringBuilder builder)
    {
        if (!DateTime.TryParse(srt, out DateTime dataResult))
        {
            Console.WriteLine("Not valid data");
            return false;
        }
        builder.Append(dataResult.ToString("dd-MM-yyyy HH:mm:ss.fff"));
        return true;
    }
    
    private bool ParseLogLevel(string srt, StringBuilder builder)
    {
        var logLevelText = srt;
        if (logLevelText.Equals(OldLogLevel.INFORMATION.ToString()))
        {
            builder.Append(LogLevel.INFO.ToString());
        }
        else if(logLevelText.Equals(OldLogLevel.WARNING.ToString()))
        {
            builder.Append(LogLevel.WARN.ToString());
        }
        else
        {
            if (!Enum.TryParse(logLevelText, out LogLevel logLevel))
            {
                Console.WriteLine("Not valid log level");
                return false;
            }
            builder.Append(logLevel.ToString());
        }

        return true;
    }
    
    private bool ParseCallFunc(string srt, StringBuilder builder)
    {
        if (srt == String.Empty)
        {
            builder.Append(DEFAULT_FUNC);
        }
        else
        {
            builder.Append(srt);
        }
        
        return true;
    }
    private bool ParseProgramVersion(string prefix, string srt, StringBuilder builder)
    {
        if (!prefix.Equals(PROGRAM_VERSION_PATTERN))
        {
            Console.WriteLine(prefix);
            Console.WriteLine("Not valid program version");
            return false;
        }
        builder.Append(PROGRAM_VERSION_PATTERN);
        builder.Append(' ');
        builder.Append(srt);
        return true;
    }

    private bool SaveToFile(string filename, string text)
    {
        try
        {
            if (!Directory.Exists(OUT_DIR))
            {
                Directory.CreateDirectory(OUT_DIR);
            }
            File.WriteAllText($"{OUT_DIR}/{filename}", text);
            Console.WriteLine($"File {filename} is saved");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot save file: {ex.Message}");
        }

        return false;
    }
}