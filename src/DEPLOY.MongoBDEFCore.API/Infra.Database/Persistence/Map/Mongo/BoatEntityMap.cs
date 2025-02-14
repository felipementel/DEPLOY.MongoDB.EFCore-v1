﻿using DEPLOY.MongoBDEFCore.API.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

public static class BoatEntityMap
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Boat>(map =>
        {
            map.AutoMap();

            map.SetIgnoreExtraElements(true);

            //map
            //.MapIdProperty(i => i.Id)
            //.SetIdGenerator(StringObjectIdGenerator.Instance);

            //map
            //.IdMemberMap
            //.SetSerializer(new StringSerializer().WithRepresentation(MongoDB.Bson.BsonType.ObjectId));

            //map.AutoMap();

            ////map.SetIgnoreExtraElements(true);

            //map
            //.MapIdProperty(i => i.Id)
            //.SetElementName("id")
            //.SetIdGenerator(StringObjectIdGenerator.Instance)
            //.SetSerializer(new StringSerializer(BsonType.ObjectId));

            //map
            //.IdMemberMap
            //.SetSerializer(new StringSerializer().WithRepresentation(MongoDB.Bson.BsonType.ObjectId));

            //map
            //.MapIdProperty(i => i.Id)
            //.SetElementName("_id")
            //.SetIdGenerator(StringObjectIdGenerator.Instance)
            //.SetSerializer(new StringSerializer(BsonType.ObjectId));

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
