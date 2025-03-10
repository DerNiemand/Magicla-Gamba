using System;
using System.Collections.Generic;

public class Shuffles
{
    public static List<T> FisherYatesShuffle<T>(List<T> list) //taken from https://www.delftstack.com/howto/csharp/shuffle-a-list-in-csharp/
    {

        int n = list.Count;

        for(int i = n - 1; i > 0; i--)
        {
            int j = Random.NextInt(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        return list;
    }

}
