namespace AgeID
{
    internal class Payload
    {
        public string salt { get; set; }
        public string encrypted { get; set; }
        public string mac { get; set; }

        public Payload ()
        {

        }
    }
}
