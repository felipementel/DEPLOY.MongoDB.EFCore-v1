using MongoDB.Bson;

namespace DEPLOY.MongoBDEFCore.API.Domain
{
    public class Boat
    {
        public string Id { get; set; } = new ObjectId().ToString();

        public string Name { get; set; }

        public double Size { get; set; }

        public bool? License { get; set; }

        public long? Version { get; set; }
    }
}
