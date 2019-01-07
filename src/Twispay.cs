using System;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public static class Twispay
{
    public static string GetBase64JsonRequest(object orderData)
    {
        return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(SerializeToJson(orderData)));
    }

    public static string GetBase64Checksum(object orderData, byte[] secretKey)
    {
        string jsonData = SerializeToJson(orderData);
        byte[] byteData = Encoding.UTF8.GetBytes(jsonData);
        // compute the checksum on the JSON text and base64 encode it
        HMACSHA512 hmacSha512 = new HMACSHA512(secretKey);
        hmacSha512.ComputeHash(byteData);
        return System.Convert.ToBase64String(hmacSha512.Hash);
    }

    private static string SerializeToJson(object o)
    {
        StringWriter writer = new StringWriter();
        var jsonWriter = new JsonTextWriter(writer);
        jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
        new JsonSerializer().Serialize(jsonWriter, o);
        return writer.ToString();
    }

    public static string GetHtmlOrderForm(object orderData, byte[] secretKey, bool twispayLive = false)
    {
        string base64JsonRequest = GetBase64JsonRequest(orderData);
        string base64Checksum = GetBase64Checksum(orderData, secretKey);
        string hostName = twispayLive ? "secure.twispay.com" : "secure-stage.twispay.com";
        return @"<form action=""https://""" + hostName + @""""" method=""post"" accept-charset=""UTF-8"">
                <input type=""hidden"" name=""jsonRequest"" value=""" + base64JsonRequest + @""">
                <input type=""hidden"" name=""checksum"" value=""" + base64Checksum + @""">
                <input type=""submit"" value=""Pay"">
            </form>";
    }

    public static object DecryptIpnResponse(string encryptedIpnResponse, byte[] secretKey)
    {
        // get the IV and the encrypted data
        string[] encryptedParts = encryptedIpnResponse.Split(",".ToCharArray(), 2);
        byte[] iv = Convert.FromBase64String(encryptedParts[0]);
        byte[] encryptedData = Convert.FromBase64String(encryptedParts[1]);
        // decrypt the encrypted data
        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
        aes.BlockSize = 128;
        aes.KeySize = 256;
        aes.IV = iv;
        aes.Key = secretKey;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.Zeros;
        byte[] decryptedIpnResponse = aes.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(decryptedIpnResponse));
    }
}
