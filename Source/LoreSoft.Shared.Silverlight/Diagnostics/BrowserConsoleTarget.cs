// allow Debug.WriteLine even in release mode
#define DEBUG

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace LoreSoft.Shared.Diagnostics
{
  [Target("BrowserConsole")]
  public class BrowserConsoleTarget : TargetWithLayout
  {
    //0 for false, 1 for true.
    private static int _scriptInjected = 0;

    private static ScriptObject _debugLog;
    private static ScriptObject _debugInfo;
    private static ScriptObject _debugWarn;
    private static ScriptObject _debugError;

    static BrowserConsoleTarget()
    {
      Deployment.Current.Dispatcher.BeginInvoke(LoadScript);
    }

    protected override void Write(LogEventInfo logEvent)
    {
      try
      {
        string message = Layout.Render(logEvent);

        // echo to debug
        Debug.WriteLine(message);

        ScriptObject scriptMethod = null;

        if (logEvent.Level == LogLevel.Info)
          scriptMethod = _debugInfo;
        else if (logEvent.Level == LogLevel.Warn)
          scriptMethod = _debugWarn;
        else if (logEvent.Level == LogLevel.Error)
          scriptMethod = _debugError;

        if (scriptMethod == null)
          scriptMethod = _debugLog;

        if (scriptMethod == null)
          return;

        // thread safe
        Deployment.Current.Dispatcher.BeginInvoke(() => 
          scriptMethod.InvokeSelf(message));
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Error calling debug.log function: " + ex);
      }
    }

    // must be called on UI thread
    public static void LoadScript()
    {
      //0 indicates that the script was not injected.
      if (0 != Interlocked.Exchange(ref _scriptInjected, 1))
        return;

      //JavaScript Debug - v0.4 - 6/22/2010
      //http://benalman.com/projects/javascript-debug-console-log/      
      //Copyright (c) 2010 "Cowboy" Ben Alman
      const string script = @"window.debug=(function(){var i=this,b=Array.prototype.slice,d=i.console,h={},f,g,m=9,c=[""error"",""warn"",""info"",""debug"",""log""],l=""assert clear count dir dirxml exception group groupCollapsed groupEnd profile profileEnd table time timeEnd trace"".split("" ""),j=l.length,a=[];while(--j>=0){(function(n){h[n]=function(){m!==0&&d&&d[n]&&d[n].apply(d,arguments)}})(l[j])}j=c.length;while(--j>=0){(function(n,o){h[o]=function(){var q=b.call(arguments),p=[o].concat(q);a.push(p);e(p);if(!d||!k(n)){return}d.firebug?d[o].apply(i,q):d[o]?d[o](q):d.log(q)}})(j,c[j])}function e(n){if(f&&(g||!d||!d.log)){f.apply(i,n)}}h.setLevel=function(n){m=typeof n===""number""?n:9};function k(n){return m>0?m>n:c.length+m<=n}h.setCallback=function(){var o=b.call(arguments),n=a.length,p=n;f=o.shift()||null;g=typeof o[0]===""boolean""?o.shift():false;p-=typeof o[0]===""number""?o.shift():n;while(p<n){e(a[p++])}};return h})();";
      
      try
      {
        HtmlWindow window = HtmlPage.Window;
        window.Eval(script);

        // get handle to methods
        _debugError = window.Eval("debug.error") as ScriptObject;
        _debugWarn = window.Eval("debug.warn") as ScriptObject;
        _debugInfo = window.Eval("debug.info") as ScriptObject;
        _debugLog = window.Eval("debug.log") as ScriptObject;
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Error loading debug.log script: " + ex);
      }
    }
  }
}
