using System;

[Serializable]
public class Logging
{
    public bool featureEnabled = false;
    public bool logToConsole = false;
    public bool logToFile = false;
    public bool logVerbose = false;
    public bool includeTimestamp = true;

    public LogType GetLogType()
    {
        LogType logType = LogType.Outline;

        if (logVerbose)
        {
            logType = LogType.Verbose;
        }

        return logType;
    }
}