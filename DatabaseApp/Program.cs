using System;
using System.Threading;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace DatabaseApp {
    public class WeatherData {
    public int ID { get; set; }
    public string city { set; get; }
    public string weather { set; get; }
    public float temperature { set; get; }
    public DateTime time { set; get; }
  }
  public class WeatherDb : DbContext {

    public virtual DbSet<WeatherData> Weather { get; set; }
  }
        
  class Program {
    const string apiKey = "a4d7d088521a6fd7b655eec8dd00e43a";
        static void ApiDownload(string city, ref JToken json)
        {
            Thread.Sleep(500);
            string url = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&units=metric&&appid=" + apiKey;
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new InvalidOperationException("Unknown city!!");
            }
            json = JToken.Parse(response.Content);
        }
        static void Main(string[] args) {
          int op;
          var context = new WeatherDb();
          do
          {
            Console.WriteLine("Option 1 - Add to database");
            Console.WriteLine("Option 2 - List all database entries");
            Console.WriteLine("Option 3 - List all database entries for selected city");
            Console.WriteLine("Option 0 - Exit");
            Console.Write("Option: ");
            op = int.Parse(Console.ReadLine());
            switch (op)
            {
              case 1:
                {
                  Console.Write("Enter city: ");
                  string city = Console.ReadLine();
                  JToken json = null;
                  Thread thread = new Thread(() =>  ApiDownload(city, ref json));
                  thread.Start();
                  thread.Join();
                    var newData = new WeatherData { city = city, weather = json.SelectToken("weather[0].main").ToString(), temperature = (float)json.SelectToken("main.temp"), time = DateTime.Now };

                    context.Weather.Add(newData);
                    context.SaveChanges();
                    break;
                  }
                case 2:
                  {
                    var weatherList = (from w in context.Weather select w).ToList();
                    foreach (var w in weatherList)
                    {
                      Console.WriteLine("City: " + w.city + "\tWeather: " + w.weather + "\tTemperature: " + w.temperature + "\tTime: " + w.time);
                    }
                    break;
                  }
                case 3:
                  {
                    Console.Write("Enter city: ");
                    string city = Console.ReadLine();
                    var weatherList = (from w in context.Weather where w.city == city select w).ToList();
                    foreach (var w in weatherList)
                    {
                      Console.WriteLine("City: " + w.city + "\tWeather: " + w.weather + "\tTemperature: " + w.temperature + "\tTime: " + w.time);
                    }
                    break;
                  }
                case 0:
                  {
                    break;
                  }
                default:
                  {
                    Console.WriteLine("Wrong option!!");
                    op = -1;
                    break;
                  }
              }
      } while (op != 0);
    }
  }
}
