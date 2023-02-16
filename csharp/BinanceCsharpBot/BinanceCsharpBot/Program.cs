using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
namespace BinanceCsharpBot
{
    public class BinanceConfig
    {
        public string StreamUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
    public class OrderBookData
    {
        public long u { get; set; }
        public string s { get; set; }
        public decimal b { get; set; }
        public decimal B { get; set; }
        public decimal a { get; set; }
        public decimal A { get; set; }
    }

    internal class Program
    {
        private static readonly ClientWebSocket WebSocket = new ClientWebSocket();
        private static bool IsOpened = false;
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var binanceConfig = configuration.GetSection("BinanceConfig").Get<BinanceConfig>();


            var cts = new CancellationTokenSource();
            //await TradeReal("BTCUSDT", "BUY", "0.00122");
          //  await StopLoss("BTCUSDT", "0.00120", "21364.0");

          // await OpenAllOrder();
            

           //var streamUrl = binanceConfig.StreamUrl + "btcusdt@bookTicker";

           // await WebSocket.ConnectAsync(new Uri(streamUrl), CancellationToken.None);

           // await ReceiveMessage(cts);

            //await Task.WhenAll(ReceiveMessage());
        }

        private static async Task ReceiveMessage(CancellationTokenSource cancellationToken)
        {
            var buffersize = 4096;

            var buffer = new ArraySegment<byte>(new byte[buffersize]);

            while (WebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count);
                    //var obj = JsonConvert.DeserializeObject<dynamic>(message)

                    OrderBookData? data = JsonConvert.DeserializeObject<OrderBookData>(message);
                    if (data == null)
                    {
                        continue;
                    }

                    Console.WriteLine("Symbol: " + data.s);
                    Console.WriteLine("Price: " + data.a);

                    var price = data.a;
                    if (price < 22980 && !IsOpened)
                    {
                        Console.WriteLine("Comprar!");

                        IsOpened = true;
                    }
                    else if (price > 23000 && IsOpened)
                    {
                        Console.WriteLine("Vender!");

                        IsOpened = false;
                    }
                }
            }
        }

        static async Task<long> GetBinanceServerTime()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("https://testnet.binance.vision/api/v1/time");
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
                    return data.serverTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return 0;
        }


        private static async Task OpenAllOrder()
        {
            string apiKey = "grQBShZqg3P5ni91VRdJu6dZttEOB1fQPxagP3e7rGrqQdwutZc8oYuRuM5QSpRN";
            string secretKey = "MAxDdVnYe61O7i75DFPTZDY5Nx7l3CpNn4qswRLur0LbzuowWMZ4G8pAjf2LRccu";
            string endpoint = "https://api.binance.com/api/v3/allOrders";


            var client = new HttpClient();
            long timestamp = await GetBinanceServerTime();
            var symbol = "BTCUSDT";

            var queryString = $"symbol={symbol}timestamp={timestamp}";

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            var signature = BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(endpoint),
                Content = new StringContent($"{queryString}&signature={signature}")
            };
            requestMessage.Headers.Add("X-MBX-APIKEY", apiKey);

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }


        private static async Task StopLoss(string symbol , string quantity , string stopPrice)
        {
            string apiKey = "grQBShZqg3P5ni91VRdJu6dZttEOB1fQPxagP3e7rGrqQdwutZc8oYuRuM5QSpRN";
            string secretKey = "MAxDdVnYe61O7i75DFPTZDY5Nx7l3CpNn4qswRLur0LbzuowWMZ4G8pAjf2LRccu";
            string endpoint = "https://api.binance.com/api/v3/order";
            string type = "STOP_LOSS_LIMIT";

            var client = new HttpClient();
            long timestamp = await GetBinanceServerTime();
            var side = "SELL";

            var queryString = $"symbol={symbol}&side={side}&type={type}&quantity={quantity}&price={stopPrice}&stopPrice={stopPrice}&timeInForce=GTC&timestamp={timestamp}";

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            var signature = BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(endpoint),
                Content = new StringContent($"{queryString}&signature={signature}")
            };
            requestMessage.Headers.Add("X-MBX-APIKEY", apiKey);

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

        }


        private static async Task CancelOpenOrders(string symbol = "BTCUSDT")
        {
            string apiKey = "grQBShZqg3P5ni91VRdJu6dZttEOB1fQPxagP3e7rGrqQdwutZc8oYuRuM5QSpRN";
            string secretKey = "MAxDdVnYe61O7i75DFPTZDY5Nx7l3CpNn4qswRLur0LbzuowWMZ4G8pAjf2LRccu";
            string endpoint = "https://api.binance.com/api/v3/openOrders";
            var client = new HttpClient();
            long timestamp = await GetBinanceServerTime();

            var queryString = $"symbol={symbol}&timestamp={timestamp}";

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            var signature = BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint),
                Content = new StringContent($"{queryString}&signature={signature}")
            };
            requestMessage.Headers.Add("X-MBX-APIKEY", apiKey);

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

        }

        private static async Task CancelOrder(string symbol = "BTCUSDT")
        {
            string apiKey = "grQBShZqg3P5ni91VRdJu6dZttEOB1fQPxagP3e7rGrqQdwutZc8oYuRuM5QSpRN";
            string secretKey = "MAxDdVnYe61O7i75DFPTZDY5Nx7l3CpNn4qswRLur0LbzuowWMZ4G8pAjf2LRccu";
            string endpoint = "https://api.binance.com/api/v3/order";
                        var client = new HttpClient();
            long timestamp = await GetBinanceServerTime();

            var queryString = $"symbol={symbol}&timestamp={timestamp}";

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            var signature = BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint),
                Content = new StringContent($"{queryString}&signature={signature}")
            };
            requestMessage.Headers.Add("X-MBX-APIKEY", apiKey);

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

        }

        private static async Task TradeReal(string symbol = "BTCUSDT", string side = "BUY", string quantity = "0.001")
        {
            string apiKey = "grQBShZqg3P5ni91VRdJu6dZttEOB1fQPxagP3e7rGrqQdwutZc8oYuRuM5QSpRN";
            string secretKey = "MAxDdVnYe61O7i75DFPTZDY5Nx7l3CpNn4qswRLur0LbzuowWMZ4G8pAjf2LRccu";
            string endpoint = "https://api.binance.com/api/v3/order";
            string type = "MARKET";

            var client = new HttpClient();
            long timestamp = await GetBinanceServerTime();

            var queryString = $"symbol={symbol}&side={side}&type={type}&quantity={quantity}&timestamp={timestamp}";

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            var signature = BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(endpoint),
                Content = new StringContent($"{queryString}&signature={signature}")
            };
            requestMessage.Headers.Add("X-MBX-APIKEY", apiKey);

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

        }

        private static async Task Trade(string symbol = "BTCUSDT", string side = "BUY", string quantity = "0.001")
        {
            string apiKey = "GRJe42ALP0pQhtdkH968cTrBXWTwGP0Qzh7ux8AdQkpNQGwsFZMAXMMj0tWIjr9Z";
            string secretKey = "gzyCFzNDOgTyO0gk3xZYjxHtcROuCc6nFxULLdbt7qKiwJzqgqgaR1WOvhvz4WgX";
            string endpoint = "https://testnet.binance.vision/api/v3/order";
            string type = "MARKET";

            var client = new HttpClient();
            long timestamp = await GetBinanceServerTime();

            var queryString = $"symbol={symbol}&side={side}&type={type}&quantity={quantity}&timestamp={timestamp}";

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            var signature = BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(endpoint),
                Content = new StringContent($"{queryString}&signature={signature}")
            };
            requestMessage.Headers.Add("X-MBX-APIKEY", apiKey);

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

        }


  


    }
}