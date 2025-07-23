namespace DEPLOY.MongoBDEFCore.API.Domain
{
    public class Boat
    {
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Size { get; set; } = 0.0;

        public bool? License { get; set; } = null;

        public long? Version { get; set; } = null;
    }
}
