using DanielWillett.ReflectionTools;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Thread-safe logger for Unturned implementing <see cref="IReflectionToolsLogger"/>.
/// </summary>
internal class UnturnedReflectionToolsLogger : IReflectionToolsLogger, IDisposable
{
    private int _sub;
    private readonly ConcurrentQueue<LogMessage> _logQueue;
    public UnturnedReflectionToolsLogger()
    {
        _logQueue = new ConcurrentQueue<LogMessage>();
        _sub = 1;

        TimeUtility.updated += OnUpdated;
    }
    private void OnUpdated()
    {
        while (_logQueue.TryDequeue(out LogMessage msg))
        {
            switch (msg.Severity)
            {
                case 0:
                case 1:
                    CommandWindow.Log(msg.Message);
                    break;
                
                case 2:
                    CommandWindow.LogWarning(msg.Message);
                    break;
                
                case 3:
                    if (msg.Message != null)
                    {
                        CommandWindow.LogError(msg.Message);
                    }
                    if (msg.Exception != null)
                    {
                        CommandWindow.LogError(msg.Exception);
                    }
                    break;
            }
        }
    }
    public void LogDebug(string source, string message)
    {
        message = $"[DBG] [{source}] {message}";
        if (Thread.CurrentThread.IsGameThread())
        {
            CommandWindow.Log(message);
        }
        else
        {
            LogMessage msg = default;
            msg.Message = message;
            msg.Severity = 0;
            _logQueue.Enqueue(msg);
        }
    }
    public void LogInfo(string source, string message)
    {
        message = $"[INF] [{source}] {message}";
        if (Thread.CurrentThread.IsGameThread())
        {
            CommandWindow.Log(message);
        }
        else
        {
            LogMessage msg = default;
            msg.Message = message;
            msg.Severity = 1;
            _logQueue.Enqueue(msg);
        }
    }
    public void LogWarning(string source, string message)
    {
        message = $"[WRN] [{source}] {message}";
        if (Thread.CurrentThread.IsGameThread())
        {
            CommandWindow.LogWarning(message);
        }
        else
        {
            LogMessage msg = default;
            msg.Message = message;
            msg.Severity = 2;
            _logQueue.Enqueue(msg);
        }
    }
    public void LogError(string source, Exception? ex, string? message)
    {
        if (message != null)
            message = $"[ERR] [{source}] {message}";
        if (Thread.CurrentThread.IsGameThread())
        {
            if (message != null)
            {
                CommandWindow.LogError(message);
            }
            if (ex != null)
            {
                CommandWindow.LogError(ex);
            }
        }
        else
        {
            LogMessage msg = default;
            msg.Message = message;
            msg.Severity = 3;
            msg.Exception = ex;
            _logQueue.Enqueue(msg);
        }
    }
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _sub, 0) > 0)
        {
            TimeUtility.updated -= OnUpdated;
        }
    }
    private struct LogMessage
    {
        public int Severity;
        public string? Message;
        public Exception? Exception;
    }
}
