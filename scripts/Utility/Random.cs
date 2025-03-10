
    public static class Random
    {
        static System.Random rng = new System.Random();
        
        public static int NextToSix() => rng.Next(0,6);

        public static bool NextBool() => (rng.Next(0,2) == 0);

        public static int NextInt(int ceiling) => rng.Next(0, ceiling);

    }

