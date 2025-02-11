using DEPLOY.MongoBDEFCore.API.Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

public static class MarinaEntityMap
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Marina>(map =>
        {
            map.AutoMap();

            map
            .MapIdProperty(i => i.Id)
            .SetElementName("_id")
            .SetIdGenerator(StringObjectIdGenerator.Instance);

            map.
            MapMember(m => m.Name)
            .SetElementName("name");
        });
    }
}