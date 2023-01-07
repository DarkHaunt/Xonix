using System;



/// <summary>
/// An instrument for comfortable random usage
/// </summary>
public static class StaticRandomizer
{
    public static readonly Random Randomizer = new Random();


    /// <returns> Or 1, or -1 </returns>
    public static int GetRandomSign() => (Randomizer.Next(0, 2) * 2) - 1;
}
