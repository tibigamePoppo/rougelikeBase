using UnityEngine;
using System.Linq;

public static class WeightRandom
{
    public static int RandomInt(int[] random,float[] weight)
    {
        float max = weight.Sum();
        float randomValue = Random.Range(0, max);
        float counter = 0;
        for (int i = 0; i < random.Length; i++)
        {
            if (counter <= randomValue && counter + weight[i] >= randomValue)
            {
                return random[i];
            }
            else
            {
                counter += weight[i];
            }
        }
        return random[0];
    }
}
