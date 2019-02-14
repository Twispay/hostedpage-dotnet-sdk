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
        // do not send empty values for optional parameters
        object orderData = new
           {
               siteId = 1, // mandatory
               customer = new // mandatory
               {
                   identifier = "external-user-id", // mandatory
                   firstName = "John ", // conditional (if required by the bank/mid)
                   lastName = "Doe", // conditional (if required by the bank/mid)
                   country = "US", // conditional (if required by the bank/mid)
                   state = "NY",
                   city = "New York", // conditional (if required by the bank/mid)
                   address = "1st Street", // conditional (if required by the bank/mid)
                   zipCode = "11222", // conditional (if required by the bank/mid)
                   phone = "0012120000000", // conditional (if required by the bank/mid)
                   email = "john.doe@test.com", // mandatory
                   tags = new string[] {
                       "customer_tag_1",
                       "customer_tag_2"
                   }
               },
               order = new
               {
                   orderId = "external-order-id", // mandatory
                   type = "recurring", // mandatory; one of: purchase, recurring, managed
                   amount = 2194.99, // mandatory
                   currency = "USD", // mandatory
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
                   intervalType = "month", // conditional (if order.type = recurring)
                   intervalValue = 1, // conditional (if order.type = recurring)
                   trialAmount = 1, // conditional (if order.type = recurring and you want smaller payment for trial)
                   firstBillDate = "2020-10-02T12:00:00+00:00", // conditional (if order.type = recurring)

                   // next fields are mandatory if your business is airlines or tourism
                   // send one of level3Airline, level3Tourism objects along with level3Type to match what you send
                   level3Type = "airline", // one of: airlines, tourism
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
                   },
                   level3Tourism = new
                   {
                       tourNumber = "8V32EU",
                       travellerName = "John Doe",
                       departureDate = "2020-02-05T14:13:00+02:00",
                       returnDate = "2020-02-06T14:13:00+02:00",
                       travelAgencyCode = "19NOV05",
                       travelAgencyName = "Elite Travel"
                   }
               },
               cardTransactionMode = "authAndCapture", // mandatory; one of: auth, authAndCapture
               cardId = 1, // optional; use it if you want to suggest customer to use one of his previous saved cards
               invoiceEmail = "john.doe@test.com", // optional; if you need different email address than of the customer's where he will receive the payment confirmation
               backUrl = "http://google.com", // optional
               customData = new // optional; any number of custom fields that you want to pass to twispay and get back on the transaction response
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

        // use base64JsonRequest and base64Checksum to generate a form
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
