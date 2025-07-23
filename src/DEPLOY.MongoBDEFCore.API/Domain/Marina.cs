using MongoDB.Bson;

namespace DEPLOY.MongoBDEFCore.API.Domain
{
    public class Marina
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public long? Version { get; set; }
    }
}
