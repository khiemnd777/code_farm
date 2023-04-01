using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

public class PythonEngine
{
  private static ScriptEngine _instance;

  public static ScriptEngine instance
  {
    get
    {
      if (_instance == null)
      {
        _instance = Python.CreateEngine();
      }

      return _instance;
    }
  }
}