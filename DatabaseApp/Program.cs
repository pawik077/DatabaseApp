using System;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DatabaseApp {
  class Program {
    const string apiKey = "a4d7d088521a6fd7b655eec8dd00e43a";
    static void Main(string[] args) {
      Console.Write("Enter city: ");
      string city = Console.ReadLine();
      string url = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&units=metric&&appid=" + apiKey;
      RestClient client = new(url);
      RestRequest request = new(Method.GET);
      IRestResponse response = client.Execute(request);
      if(!response.IsSuccessful) {
        Console.Error.WriteLine("Unknown city!!");
        return;
      }
      JToken json = JToken.Parse(response.Content);
      Console.WriteLine("Current weather in " + city + ":");
      Console.WriteLine("Weather: " + json.SelectToken("weather[0].main"));
      Console.WriteLine("Current temperature: " + json.SelectToken("main.temp") + "\u00B0C");
    }
  }
}
