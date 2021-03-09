
namespace ProxyAPI
{
    public static class Random
    {
        private static System.Random random = new System.Random();
        public static int RandomNumber(int range)
        {
            return random.Next(range);
        }
    }
}