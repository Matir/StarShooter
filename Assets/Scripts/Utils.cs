using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class Utils {
    private static Random rng = new Random();

    // Get a random element from a list
    public static T GetRandomElement<T>(IList<T> list) {
        int n = rng.Next(list.Count);
        return list[n];
    }

    // Convert an enum to a list of choices.
    public static List<T> EnumToList<T>() {
        T[] items = (T[])Enum.GetValues(typeof(T));
        return new List<T>(items);
    }
}
