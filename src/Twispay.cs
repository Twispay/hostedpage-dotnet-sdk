using System;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/*
The Twispay class implements methods to get the value
of `jsonRequest` and `checksum` that need to be sent by POST
when making a Twispay order and to decrypt the Twispay IPN response.
*/
/// <summary>
/// The <c>Twispay</c> class implements methods to get the value
/// of `jsonRequest` and `checksum` that need to be sent by POST
/// when making a Twispay order and to decrypt the Twispay IPN response.
/// </summary>
public static class Twispay
{
    // Get the `jsonRequest` parameter (order parameters as JSON and base64 encoded).
    /// <summary>
    /// Get the `jsonRequest` parameter (order parameters as JSON and base64 encoded).
    /// </summary>
    /// <returns>
    /// The `jsonRequest` parameter as string.
    /// </returns>
    /// <param name="orderData">The order parameters.</param>
    public static string GetBase64JsonRequest(object orderData)
    {
        return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(SerializeToJson(orderData)));
    }

    // Get the `checksum` parameter (the checksum computed over the `jsonRequest` and base64 encoded).
    /// <summary>
    /// Get the `checksum` parameter (the checksum computed over the `jsonRequest` and base64 encoded).
    /// </summary>
    /// <returns>
    /// The `checksum` parameter as string.
    /// </returns>
    /// <param name="orderData">The order parameters.</param>
    /// <param name="secretKey">The secret key (from Twispay).</param>
    public static string GetBase64Checksum(object orderData, byte[] secretKey)
    {
        string jsonData = SerializeToJson(orderData);
        byte[] byteData = Encoding.UTF8.GetBytes(jsonData);
        // compute the checksum on the JSON text and base64 encode it
        HMACSHA512 hmacSha512 = new HMACSHA512(secretKey);
        hmacSha512.ComputeHash(byteData);
        return System.Convert.ToBase64String(hmacSha512.Hash);
    }

    // Serialize object to JSON.
    /// <summary>
    /// Serialize object to JSON.
    /// </summary>
    /// <returns>
    /// The serialized object as JSON string.
    /// </returns>
    /// <param name="o">The object to be serialized.</param>
    private static string SerializeToJson(object o)
    {
        StringWriter writer = new StringWriter();
        var jsonWriter = new JsonTextWriter(writer);
        jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
        new JsonSerializer().Serialize(jsonWriter, o);
        return writer.ToString();
    }

    // Decrypt the IPN response from Twispay.
    /// <summary>
    /// Decrypt the IPN response from Twispay.
    /// </summary>
    /// <returns>
    /// The descrypted IPN response as object.
    /// </returns>
    /// <param name="encryptedIpnResponse">The encrypted IPN response.</param>
    /// <param name="secretKey">The secret key (from Twispay).</param>
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
