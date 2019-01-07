using System;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        object orderData = TwispaySample.GetOrderData();

        // your secret key
        string secretKey = TwispaySample.GetSecretKey();

        if (args.Length == 2)
        {
            Console.WriteLine("Arguments provided for JSON order data and secret key.");
            orderData = args[0];
            secretKey = args[1];
        }
        else
        {
            Console.WriteLine("No arguments provided for JSON order data and secret key, using sample values!");
        }

        Console.WriteLine("jsonOrderData: " + orderData);
        Console.WriteLine("secretKey: " + secretKey);

        // get the HTML form
        String htmlForm = Twispay.GetHtmlOrderForm(orderData, Encoding.ASCII.GetBytes(secretKey));

        Console.Write(htmlForm);
    }
}
