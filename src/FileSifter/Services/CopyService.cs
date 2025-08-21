namespace FileSifter.Services;

public sealed class CopyService
{
    public void CopyFile(string source, string dest, string policy)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);

        if (File.Exists(dest))
        {
            switch (policy)
            {
                case "overwrite":
                    File.Copy(source, dest, true);
                    return;
                case "skip":
                    return;
                case "rename":
                    dest = NextAvailable(dest);
                    File.Copy(source, dest);
                    return;
            }
        }
        else
        {
            File.Copy(source, dest);
        }
    }

    private string NextAvailable(string path)
    {
        var dir = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);
        int i = 2;
        string candidate;
        do
        {
            candidate = Path.Combine(dir, $"{name}_({i}){ext}");
            i++;
        } while (File.Exists(candidate));
        return candidate;
    }
}