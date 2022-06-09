using System;
using System.Collections.Generic;

namespace oreprocessing;

public static class ProportionalWheelSelection
{
    public static readonly Random rnd = new Random();

    public static int SelectItem(List<thingMined> things)
    {
        var num = 0;
        foreach (var thingMined in things)
        {
            num += thingMined.chance;
        }

        var num2 = rnd.Next(0, num) + 1;
        var num3 = 0;
        for (var j = 0; j < things.Count; j++)
        {
            num3 += things[j].chance;
            if (num2 <= num3)
            {
                return j;
            }
        }

        return 0;
    }
}