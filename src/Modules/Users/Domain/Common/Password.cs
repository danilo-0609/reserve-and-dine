using System.Security.Cryptography;
using System.Text;

namespace Users.Domain.Common;

public sealed class Password
{
    public string Value { get; private set; } = string.Empty;

    public static Password Create(string password)
    {
        return new Password(password);
    }

    public static Password CreateUnique(string value)
    {
        string hash = GenerateHash256(value);

        return new Password(hash);
    }

    private static string GenerateHash256(string value)
    {
        using SHA256 sha256 = SHA256.Create();

        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));

        StringBuilder password = new StringBuilder();

        foreach (byte d in bytes)
        {
            password.Append(d.ToString());
        }

        return password.ToString();
    }

    private Password(string value)
    {
        Value = value;
    }
}
