using System.Text;

namespace DeepTrace.ML;

public static class StreamUtils
{
    private static readonly int SizeOfInt = BitConverter.GetBytes(1).Length;

    public static Stream WriteInt(this Stream stream, int value)
    {
        stream.Write(BitConverter.GetBytes(value));
        return stream;
    }

    public static int ReadInt(this Stream stream)
    {
        var buffer= new byte[SizeOfInt];
        stream.Read(buffer, 0, SizeOfInt);
        return BitConverter.ToInt32(buffer);
    }

    public static Stream WriteString(this Stream stream, string value)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        stream.WriteInt(utf8.Length);
        stream.Write(utf8);

        return stream;
    }

    public static string ReadString(this Stream stream)
    {
        var len = stream.ReadInt();
        var utf8 = new byte[len];
        stream.Read(utf8, 0, len);

        return Encoding.UTF8.GetString(utf8);
    }
}
