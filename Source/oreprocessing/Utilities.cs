using System;

namespace oreprocessing;

public static class Utilities
{
    public static int RandomNumber(int min, int max)
    {
        var random = new Random();
        return random.Next(min, max);
    }
}