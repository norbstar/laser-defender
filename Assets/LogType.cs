using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LogType
{
    public static LogType Outline { get; } = new LogType(1, "Outline");
    public static LogType Verbose { get; } = new LogType(2, "Verbose");

    public int Ordinal { get; private set; }
    public string Name { get; private set; }

    public LogType(int ordinal, string name)
    {
        Ordinal = ordinal;
        Name = name;
    }

    public static IEnumerable<LogType> List()
    {
        return new[] { Outline, Verbose };
    }

    public static LogType FromString(string name)
    {
        return List().Single(lt => String.Equals(lt.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public static LogType FromIndex(int ordinal)
    {
        return List().Single(lt => lt.Ordinal == ordinal);
    }

    public bool Featured(LogType logType)
    {
        return (logType.Ordinal <= Ordinal);
    }
}