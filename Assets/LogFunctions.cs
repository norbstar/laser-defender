using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class LogFunctions
{
    public enum LogSpecial
    {
        NEWLINE
    }

    public static string FloatFormat { get; } = "0.0000";

    private static string GetLastMessage(string context)
    {
        string lastMessage = null;

        if (File.Exists($"{context}.txt"))
        {
            IEnumerable<string> messages = File.ReadLines($"{context}.txt");

            int count = 0;

            using (IEnumerator<string> enumerator = messages.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }

            if (count > 0)
            {
                lastMessage = File.ReadLines($"{context}.txt").Last();
            }
        }

        return lastMessage;
    }

    private static float? ExtractTimestamp(string message)
    {
        float? timestamp = null;

        if (message != null)
        {
            var match = Regex.Match(message, @"([-+]?[0-9]*\.?[0-9]+)");

            if (match.Success)
            {
                timestamp = Convert.ToSingle(match.Groups[1].Value);
            }
        }

        return timestamp;
    }

    public static void Log(string context, string message, Logging logging = null)
    {
        if (Application.isEditor)
        {
            float? timestamp = null;
            bool logToFile = true;
            bool logToConsole = true;
            bool includeTimestamp = true;

            if (logging != null)
            {
                logToConsole = logging.logToConsole;
                logToFile = logging.logToFile;
                includeTimestamp = logging.includeTimestamp;
            }

            if (includeTimestamp)
            {
                timestamp = Time.time;
            }

            if (logToConsole)
            {
                LogToConsole(timestamp, context, message);
            }

            if (logToFile)
            {
                LogToFile(timestamp, context, message);
            }
        }
    }

    public static void Delete(string context)
    {
        File.Delete($"{context}.txt");
    }

    public static void LogToFile(string context, string message)
    {
        LogToFile(Time.time, context, message);
    }
    public static void LogRawToFile(string filename, string message)
    {
        StreamWriter sw = new StreamWriter(filename, true);
        sw.WriteLine(message);
        sw.Close();
    }

    private static void LogToFile(float? timestamp, string context, string message)
    {
        float? lastTimestamp = ExtractTimestamp(GetLastMessage(context));

        StreamWriter sw = new StreamWriter($"{context}.txt", true);

        if (timestamp.HasValue)
        {
            if (lastTimestamp != null)
            {
                sw.WriteLine($"{timestamp.Value.ToString(LogFunctions.FloatFormat)} [{((float) timestamp - (float) lastTimestamp).ToString(LogFunctions.FloatFormat)}] {message}");
            }
            else
            {
                sw.WriteLine($"{timestamp.Value.ToString(LogFunctions.FloatFormat)} {message}");
            }
        }
        else
        {
            sw.WriteLine($"{message}");
        }

        sw.Close();
    }

    public static void LogToConsole(string context, string message)
    {
        LogToConsole(Time.time, context, message);
    }

    private static void LogToConsole(float? timestamp, string context, string message)
    {
        if (timestamp.HasValue)
        {
            Debug.Log($"{(timestamp.Value).ToString(LogFunctions.FloatFormat)} {context} {message}");
        }
        else
        {
            Debug.Log($"{context} {message}");
        }
    }
}