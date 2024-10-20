using System.Diagnostics;

public class IDE
{
    private Process ide;

    public void Open(string file)
    {
        if (ide != null)
        {
            Close();
        }

        var info = new ProcessStartInfo
        {
            FileName = @"C:\Works\project_code-farm\ide\ScintillaNET.Demo\ScintillaNET.Demo\bin\Debug\ScintillaNET.Demo.exe",
            Arguments = $"{file}",
            UseShellExecute = true
        };
        ide = Process.Start(info);
    }


    public void Close()
    {
        try
        {
            if (ide != null)
            {
                ide.Kill();
                ide = null;
            }
        }
        catch
        {
            ide = null;
        }
    }
}
