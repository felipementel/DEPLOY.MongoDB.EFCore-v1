﻿namespace DEPLOY.MongoBDEFCore.API.Domain
{
    public class Boat
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Size { get; set; }

        public bool? License { get; set; }

        public long? Version { get; set; }
    }
}
