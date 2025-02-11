using MongoDB.Bson;

namespace DEPLOY.MongoBDEFCore.API.Domain
{
    public class Boat
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public double Size { get; set; }

        public bool? License { get; set; }

        public long? version { get; set; }
    }
}
