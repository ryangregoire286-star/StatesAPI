using APIStates.States;
using Carter;
using DB;
using Init;
using Microsoft.AspNetCore.Builder;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Init
{
    public interface IGetClientOn
    {
        string ClientOn(string clientMessage);
        string SetupClient(string init);
    }

    public class Clients : IGetClientOn
    {
        public string ClientOn(string clientMessage)
        {
            return SetupClient(clientMessage);
        }

        public string SetupClient(string init)
        {
            return init.ToUpper();
        }
    }
    
}

namespace DB
{

    public abstract class DbMongo
    {
        private static string URi(string uri)
        {
            return uri.ToLower();
        }

        public static void Connect(string? dataState)
        {
            var db = URi("mongodb://localhost:27017");

            var mongoClient = new MongoClient(db);

            var dbClient = GetDb("states");
            var dbDb = mongoClient.GetDatabase(dbClient);
            var coll = dbDb.GetCollection<BsonDocument>("state");
            coll.InsertOne(new BsonDocument("state", dataState));
        }

        private static string GetDb(string db)
        {
            return db.ToUpper();
        }
    }
}

namespace APIStates
{
    internal abstract class Program
    {
        
        private delegate void CreateDelagates(string[] args);
        
        public static void GetDelagates(string[] args)
        {

            var clients = new Clients();
            
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCarter();
            var app = builder.Build();
            app.MapCarter();
            
            app.MapGet("/", () => clients.ClientOn("Hello I am Ryan The Developer"));
            app.Run();
        }

        public static void Main(string[] args)
        {
            if (!args.Length.Equals(0)) Console.WriteLine("No Args Found");
            try
            {
                var state = State.States;
                var city = State.City;
                
                // List some USA States Not Done Yet
                state.Add("NH");
                state.Add("MA");
                state.Add("NY");

                // List some CITY'S IN NEW HAMPSHIRE But Not Done Yet
                city.Add("Nashua");
                city.Add("Manchester");
                city.Add("Concord");
                
                CreateDelagates run = GetDelagates;
                DbMongo.Connect(state.ToString());
                run(args);
            }
            catch (Exception ce)
            {
                Console.Write(ce.Message);
            }
            
        }
    }
}