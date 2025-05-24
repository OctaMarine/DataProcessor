using System.Text;

namespace DataProcessor;

public class Compression
{
    public string Compress(string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length <= 1)
        {
            return str;
        }

        var builder = new StringBuilder();
        var stepChar = str[0];
        var stepCount = 1;

        for (int i = 1; i < str.Length; i++)
        {
            if (str[i] == stepChar)
            {
                stepCount++;
            }
            else
            {
                //при нахождении следующего символа записываем результат
                builder.Append(stepChar);
                if (stepCount > 1)
                {
                    builder.Append(stepCount);
                }

                stepChar = str[i];
                stepCount = 1;
            }
        }
        // записывает последний остаток в результат
        builder.Append(stepChar);
        if (stepCount > 1)
        {
            builder.Append(stepCount);
        }
        

        return builder.ToString();
    }

    public string Decompress(string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length <= 1)
        {
            return str;
        }
        
        var builder = new StringBuilder();
        var stepIndex = 0;
        while (true)
        {
            var stepChar = str[stepIndex];
            if (stepIndex+1 >= str.Length)
            {
                builder.Append(stepChar);
                break;
            }
            if (int.TryParse(str[stepIndex + 1].ToString(), out int count))
            {
                //добавляем нужное количество одинаковых символов в результат
                builder.Append(stepChar,count);
                stepIndex += 2;
                if (stepIndex >= str.Length)
                {
                    break;
                }
            }
            else
            {
                builder.Append(stepChar);
                stepIndex++;
                if (stepIndex >= str.Length)
                {
                    break;
                }
            }
        }

        return builder.ToString();
    }
}