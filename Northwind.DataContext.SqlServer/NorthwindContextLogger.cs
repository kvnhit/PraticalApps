using static System.Environment;

namespace Northwind.EntityModels;

public class NorthwindContextLogger
{
    public static void WriteLine(string message)
    {
        string path = Path.Combine(GetFolderPath(SpecialFolder.DesktopDirectory), "northwindlog.txt");

        StreamWriter textFile = new StreamWriter(path);
        textFile.WriteLine(message);
        textFile.Close();
    }
}
