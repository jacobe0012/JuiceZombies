using HotFix_UI;
using MessagePack;
using Newtonsoft.Json;

namespace JuiceZombies.Server.Utility;

public class MyHelper
{
    public static byte[] SerializeMessagePack<T>(string value)
    {
        var options =
            MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

        return MessagePackSerializer.Serialize(JsonConvert.DeserializeObject<T>(value), options);
    }
    
    public static (string, string) GetInputOutPutStr(object input, object output)
    {
        var inputContentStr = JsonConvert.SerializeObject(input);
        var outputContentStr = JsonConvert.SerializeObject(output);
        return (inputContentStr, outputContentStr);
    }
}