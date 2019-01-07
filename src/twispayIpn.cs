using System;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        // normally you get the encrypted data from the HTTP request (POST/GET) in the `opensslResult` parameter
        string encryptedIpnResponse = TwispaySample.GetEncryptedIpnResponse();

        // your secret key
        string secretKey = TwispaySample.GetSecretKey();

        if (args.Length == 2)
        {
            Console.WriteLine("Arguments provided for JSON order data and secret key.");
            encryptedIpnResponse = args[0];
            secretKey = args[1];
        }
        else
        {
            Console.WriteLine("No arguments provided for JSON order data and secret key, using sample values!");
        }

        Console.WriteLine("encryptedIpnResponse: " + encryptedIpnResponse);
        Console.WriteLine("secretKey: " + secretKey);

        // get the IPN response
        object response = Twispay.DecryptIpnResponse(encryptedIpnResponse, Encoding.ASCII.GetBytes(secretKey));

        Console.Write(response);
    }
}
