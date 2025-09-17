using System.IO;
using System.Security.Cryptography;

namespace FileSifter.Services;

public sealed class HashService
{
    private readonly string _algorithm; // "xxhash64" | "sha256"
    private readonly byte[] _buffer;

    public HashService(string algorithm)
    {
        _algorithm = algorithm.ToLowerInvariant();
        _buffer = new byte[64 * 1024];
    }

    public (ulong? xx, byte[]? sha) Compute(string path)
    {
        if (_algorithm == "xxhash64")
            return (ComputeXxHash(path), null);
        if (_algorithm == "sha256")
            return (null, ComputeSha256(path));
        // fallback
        return (ComputeXxHash(path), null);
    }

    private ulong ComputeXxHash(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, FileOptions.SequentialScan);
        var xxh64 = new K4os.Hash.xxHash.XXH64();
        int read;
        while ((read = fs.Read(_buffer, 0, _buffer.Length)) > 0)
            xxh64.Update(_buffer.AsSpan(0, read));
        return xxh64.Digest();
    }

    private byte[] ComputeSha256(string path)
    {
        using var sha = SHA256.Create();
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, FileOptions.SequentialScan);
        return sha.ComputeHash(fs);
    }
}