using System;
using System.Text;

/*
 * Example code for generating a HTML form to be posted to Twispay.
 */
public class Program
{
    public static void Main(string[] args)
    {
        // sample data contains all available parameters
        // depending on order type, not all parameters are required/needed
        // you need to replace `siteId` etc. with valid data
        object orderData = new
           {
               siteId = 1,
               customer = new
               {
                   identifier = "external-user-id",
                   firstName = "John ",
                   lastName = "Doe",
                   country = "US",
                   state = "NY",
                   city = "New York",
                   address = "1st Street",
                   zipCode = "11222",
                   phone = "0012120000000",
                   email = "john.doe@test.com",
                   tags = new string[] {
                       "customer_tag_1",
                       "customer_tag_2"
                   }
               },
               order = new
               {
                   orderId = "external-order-id",
                   type = "recurring",
                   amount = 2194.99,
                   currency = "USD",
                   items = new object[] {
                       new {
                           item = "1 year subscription on site",
                           unitPrice = 34.99,
                           units = 1,
                           type = "digital",
                           code = "xyz",
                           vatPercent = 19,
                           itemDescription = "1 year subscription on site"
                       },
                       new {
                           item = "200 tokens",
                           unitPrice = 10.75,
                           units = 200,
                           type = "digital",
                           code = "abc",
                           vatPercent = 19,
                           itemDescription = "200 tokens"
                       },
                       new {
                           item = "discount",
                           unitPrice = 10,
                           units = 1,
                           type = "digital",
                           code = "fgh",
                           vatPercent = 19,
                           itemDescription = "discount"
                       }
                   },
                   tags = new string[] {
                       "tag_1",
                       "tag_2"
                   },
                   intervalType = "month",
                   intervalValue = 1,
                   trialAmount = 1,
                   firstBillDate = "2020-10-02T12:00:00+00:00",
                   level3Airline = new
                   {
                       ticketNumber = "8V32EU",
                       passengerName = "John Doe",
                       flightNumber = "SQ619",
                       departureDate = "2020-02-05T14:13:00+02:00",
                       departureAirportCode = "KIX",
                       arrivalAirportCode = "OTP",
                       carrierCode = "American Airlines",
                       travelAgencyCode = "19NOV05",
                       travelAgencyName = "Elite Travel"
                   }
               },
               cardTransactionMode = "authAndCapture",
               cardId = 1,
               invoiceEmail = "john.doe@test.com",
               backUrl = "http://google.com",
               customData = new
               {
                   key1 = "value",
                   key2 = "value"
               }
           };

        // your secret key
        string secretKey = "cd07b3c95dc9a0c8e9318b29bdc13b03";

        Console.WriteLine("jsonOrderData: " + orderData);
        Console.WriteLine("secretKey: " + secretKey);

        // TRUE for Twispay live site, otherwise Twispay stage will be used
        bool twispayLive = false;

        // get the HTML form
        string base64JsonRequest = Twispay.GetBase64JsonRequest(orderData);
        string base64Checksum = Twispay.GetBase64Checksum(orderData, Encoding.ASCII.GetBytes(secretKey));
        string hostName = twispayLive ? "secure.twispay.com" : "secure-stage.twispay.com";
        string htmlForm = @"<form action=""https://""" + hostName + @""""" method=""post"" accept-charset=""UTF-8"">
            <input type=""hidden"" name=""jsonRequest"" value=""" + base64JsonRequest + @""">
            <input type=""hidden"" name=""checksum"" value=""" + base64Checksum + @""">
            <input type=""submit"" value=""Pay"">
            </form>";

        Console.Write("Generated HTML form: " + htmlForm);
    }
}
