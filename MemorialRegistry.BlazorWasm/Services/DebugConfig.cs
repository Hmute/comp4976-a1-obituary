/* 
 * DEBUG CONFIGURATION SYSTEM
 * ==========================
 * 
 * 🔧 CENTRALIZED DEBUG CONTROL
 * 
 * This file controls all debug features throughout the Blazor WebAssembly application.
 * Simply uncomment the sections you want to enable for development/troubleshooting.
 * 
 * Usage:
 * - Components inject DebugConfig and check IsEnabled properties
 * - JavaScript utilities are conditionally loaded
 * - Console logging is controlled centrally
 * - Debug panels show/hide based on these settings
 */

namespace MemorialRegistry.BlazorWasm.Services;

public class DebugConfig
{
    // 🚨 MASTER DEBUG SWITCH - Uncomment to enable ALL debug features
    // public static bool IsDebugMode => true;
    public static bool IsDebugMode => false;

    // 🎛️ INDIVIDUAL DEBUG FEATURES - Uncomment specific features as needed
    
    // Debug Panel Visibility
    // public static bool ShowDebugPanel => true;
    public static bool ShowDebugPanel => false;
    
    // Debug Button in UI
    // public static bool ShowDebugButton => true;
    public static bool ShowDebugButton => false;
    
    // Console Logging
    // public static bool EnableConsoleLogging => true;
    public static bool EnableConsoleLogging => false;
    
    // API Call Logging
    // public static bool LogApiCalls => true;
    public static bool LogApiCalls => false;
    
    // Performance Monitoring
    // public static bool EnablePerformanceMonitoring => true;
    public static bool EnablePerformanceMonitoring => false;
    
    // Keyboard Shortcuts (Ctrl+D)
    // public static bool EnableKeyboardShortcuts => true;
    public static bool EnableKeyboardShortcuts => false;
    
    // Error Details (show stack traces, etc.)
    // public static bool ShowDetailedErrors => true;
    public static bool ShowDetailedErrors => false;
    
    // Component Render Logging
    // public static bool LogComponentRenders => true;
    public static bool LogComponentRenders => false;

    // 🔧 HELPER METHODS
    
    /// <summary>
    /// Check if ANY debug feature is enabled
    /// </summary>
    public static bool IsAnyDebugEnabled => 
        IsDebugMode || ShowDebugPanel || ShowDebugButton || 
        EnableConsoleLogging || LogApiCalls || EnablePerformanceMonitoring ||
        EnableKeyboardShortcuts || ShowDetailedErrors || LogComponentRenders;
    
    /// <summary>
    /// Log message if console logging is enabled
    /// </summary>
    public static void Log(string message)
    {
        if (EnableConsoleLogging || IsDebugMode)
        {
            Console.WriteLine($"🔧 DEBUG: {message}");
        }
    }
    
    /// <summary>
    /// Log API call if API logging is enabled
    /// </summary>
    public static void LogApiCall(string method, string url, string status = "")
    {
        if (LogApiCalls || IsDebugMode)
        {
            Console.WriteLine($"🌐 API: {method} {url} {status}");
        }
    }
    
    /// <summary>
    /// Log performance metric if performance monitoring is enabled
    /// </summary>
    public static void LogPerformance(string operation, TimeSpan duration)
    {
        if (EnablePerformanceMonitoring || IsDebugMode)
        {
            Console.WriteLine($"📊 PERF: {operation} took {duration.TotalMilliseconds:F2}ms");
        }
    }
    
    /// <summary>
    /// Log component render if component logging is enabled
    /// </summary>
    public static void LogComponentRender(string componentName)
    {
        if (LogComponentRenders || IsDebugMode)
        {
            Console.WriteLine($"🎨 RENDER: {componentName}");
        }
    }
}

/* 
 * 🚀 QUICK ENABLE GUIDE
 * =====================
 * 
 * FOR GENERAL DEBUGGING:
 * ----------------------
 * Uncomment: public static bool IsDebugMode => true;
 * 
 * FOR SPECIFIC FEATURES:
 * ----------------------
 * Uncomment individual properties as needed
 * 
 * FOR API TROUBLESHOOTING:
 * ------------------------
 * Uncomment: LogApiCalls, EnableConsoleLogging, ShowDetailedErrors
 * 
 * FOR UI DEBUGGING:
 * -----------------
 * Uncomment: ShowDebugPanel, ShowDebugButton, EnableKeyboardShortcuts
 * 
 * FOR PERFORMANCE ANALYSIS:
 * -------------------------
 * Uncomment: EnablePerformanceMonitoring, LogComponentRenders
 */