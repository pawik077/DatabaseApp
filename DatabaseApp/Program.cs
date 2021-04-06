using System;
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
  }
  public class WeatherDb : DbContext {

    public virtual DbSet<WeatherData> Weather { get; set; }
  }

  class Program {
    const string apiKey = "a4d7d088521a6fd7b655eec8dd00e43a";
    static void Main(string[] args) {
      
      Console.Write("Enter city: ");
      string city = Console.ReadLine();
      string url = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&units=metric&&appid=" + apiKey;
      RestClient client = new RestClient(url);
      RestRequest request = new RestRequest(Method.GET);
      IRestResponse response = client.Execute(request);
      if(!response.IsSuccessful) {
        Console.Error.WriteLine("Unknown city!!");
        return;
      }
      JToken json = JToken.Parse(response.Content);
      //Console.WriteLine("Current weather in " + city + ":");
      //Console.WriteLine("Weather: " + json.SelectToken("weather[0].main"));
      //Console.WriteLine("Current temperature: " + json.SelectToken("main.temp") + "\u00B0C");
      var context = new WeatherDb();
      var newData = new WeatherData {  city = city, weather = json.SelectToken("weather[0].main").ToString(), temperature = (float)json.SelectToken("main.temp")  };
      
      context.Weather.Add(newData);
      context.SaveChanges();
      //var weatherList = context.Weather.SqlQuery("SELECT * FROM Weather").ToList();
      var weatherList = (from w in context.Weather select w).ToList();
      foreach (var w in weatherList) {
        Console.WriteLine("City: " + w.city + "\tWeather: " + w.weather + "\tTemperature: " + w.temperature);
      }
      Console.Read();
    }
  }
}
