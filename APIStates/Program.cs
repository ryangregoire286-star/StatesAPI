using Carter;
using DB;
using Init;
using Microsoft.AspNetCore.Builder;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AsyncId
{
    internal abstract class GetSyncTask
    {
        public static async Task GetTask()
        {
            Thread.Sleep(2 * 1000);
            SendTask();
            await Task.CompletedTask;
        }

        private static void SendTask()
        {
            Console.WriteLine("Task Completed");
        }
    }
}

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

        public static async Task Connect(string? dataState)
        {
            var db = URi("mongodb://localhost:27017");

            var mongoClient = new MongoClient(db);

            var dbClient = GetDb("states");
            var dbDb = mongoClient.GetDatabase(dbClient);
            var coll = dbDb.GetCollection<BsonDocument>("state");
            await coll.InsertOneAsync(new BsonDocument("state", dataState));

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

        public static async Task Main(string[] args)
        {
            if (!args.Length.Equals(0)) Console.WriteLine("No Args Found");
            try
            {
                string[] states = ["NYC", "MA", "NH"];


                foreach (var st in states)
                {
                    await DbMongo.Connect(st);
                    Console.WriteLine(states);
                }

                var dict1 = new Dictionary<string, int>
                {
                    { "Nashua", 0 },
                    { "Manchester", 1 },
                    { "Concord", 2 }
                };

                foreach (var s in dict1)
                {
                    var id = s.Key + " " + Convert.ToInt16(s.Value);
                    await DbMongo.Connect(id);
                }

                CreateDelagates run1 = GetDelagates;
                run1(args);

                // List some CITY'S IN NEW HAMPSHIRE But Not Done Ye
                //

                await AsyncId.GetSyncTask.GetTask();
            }

            catch (Exception ce)
            {
                Console.Write(ce.Message);
            }
        }
    }
}