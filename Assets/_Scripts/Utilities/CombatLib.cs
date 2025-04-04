using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine;

public static class CombatLib
{
    private static readonly System.Random random = new();

    public static int RollDice(int sides = 6)
    {
        if (sides <= 0)
        {
            throw new ArgumentException("Number of sides must be greater than zero.");
        }

        return random.Next(1, sides + 1);
    }


    public static int RollXWY(int diceCount, int sides = 6)
    {
        if (diceCount <= 0)
        {
            throw new ArgumentException("Number of dice must be greater than zero.");
        }

        int total = 0;
        for (int i = 0; i < diceCount; i++)
        {
            total += RollDice(sides);
        }

        return total;
    }

}