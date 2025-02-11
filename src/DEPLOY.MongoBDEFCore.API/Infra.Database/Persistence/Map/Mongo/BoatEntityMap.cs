using DEPLOY.MongoBDEFCore.API.Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

public static class BoatEntityMap
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Boat>(map =>
        {
            map.AutoMap();

            //map.SetIgnoreExtraElements(true);

            map
            .MapIdProperty(i => i.Id)
            .SetElementName("_id")
            .SetIdGenerator(StringObjectIdGenerator.Instance);

            map.
            MapMember(m => m.Name)
            .SetElementName("name");

            map.
            MapMember(m => m.Size)
            .SetElementName("size");

            map.
            MapMember(m => m.License)
            .SetElementName("license");
        });
    }
}
