using DEPLOY.MongoBDEFCore.API.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

public static class MarinaEntityMap
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Marina>(map =>
        {
            map.AutoMap();

            map
            .MapIdProperty(i => i.Id)
            .SetElementName("_id");
            //.SetIdGenerator(StringObjectIdGenerator.Instance)
            //.SetSerializer(new StringSerializer(BsonType.ObjectId));

            //map
            //.MapIdProperty(i => i.Id)
            //.SetElementName("_id")
            //.SetIdGenerator(StringObjectIdGenerator.Instance)
            //.SetSerializer(new StringSerializer(BsonType.ObjectId));

            map.
            MapMember(m => m.Name)
            .SetElementName("name");
        });
    }
}