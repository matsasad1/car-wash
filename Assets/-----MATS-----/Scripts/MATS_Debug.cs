using UnityEngine;
using System.Text;

public static class MATS_Debug
{
    // Master switch (disable logs in production builds)
     public static bool EnableLogging = true;

    // Settings
    private static readonly string InfoColor = "cyan";
    private static readonly string WarningColor = "yellow";
    private static readonly string ErrorColor = "red";
    private static readonly string SuccessColor = "lime";
    private static readonly string CriticalColor = "magenta";
    private static readonly StringBuilder builder = new StringBuilder();

    private static bool ShowTimestamp = true;
    private static bool ShowStackTrace = false;

    #region Public Methods

    public static void Log(string message, string tag = "INFO", Object context = null)
    {
        if (!EnableLogging) return;
        Debug.Log(Format(message, tag, InfoColor), context);
    }

    public static void LogWarning(string message, string tag = "WARNING", Object context = null)
    {
        if (!EnableLogging) return;
        Debug.LogWarning(Format(message, tag, WarningColor), context);
    }

    public static void LogError(string message, string tag = "ERROR", Object context = null)
    {
        if (!EnableLogging) return;
        Debug.LogError(Format(message, tag, ErrorColor), context);
    }

    public static void LogSuccess(string message, string tag = "SUCCESS", Object context = null)
    {
        if (!EnableLogging) return;
        Debug.Log(Format(message, tag, SuccessColor), context);
    }

    public static void LogCritical(string message, string tag = "CRITICAL", Object context = null)
    {
        if (!EnableLogging) return;
        Debug.LogError(Format(message, tag, CriticalColor), context);
    }

    public static void LogCustom(string message, string tag = "CUSTOM", string color = "white", Object context = null)
    {
        if (!EnableLogging) return;
        Debug.Log(Format(message, tag, color), context);
    }

    #endregion

    #region Helpers

    private static string Format(string message, string tag, string color)
    {
        builder.Clear();

        if (ShowTimestamp)
        {
            builder.Append('[').Append(System.DateTime.Now.ToString("HH:mm:ss")).Append("] ");
        }

        builder.Append("<b><color=").Append(color).Append(">[")
               .Append(tag).Append("]</color></b> ")
               .Append(message);

        if (ShowStackTrace)
        {
            builder.Append("\n<color=grey>")
                   .Append(new System.Diagnostics.StackTrace(2, true))
                   .Append("</color>");
        }

        return builder.ToString();
    }

    #endregion
}
