using System;
using System.Collections.Generic;

// Test double: returns a pre-set sequence of rolls in order.
// Throws if it runs out of rolls or a scripted roll is impossible for the die being rolled.
public class ScriptedDie : Die
{
    private readonly Queue<int> _rolls;

    public ScriptedDie(params int[] rolls)
    {
        _rolls = new Queue<int>(rolls);
    }

    public override int Roll(int sides)
    {
        if (_rolls.Count == 0)
            throw new InvalidOperationException("ScriptedDie ran out of scripted rolls.");

        int roll = _rolls.Dequeue();

        if (roll < 1 || roll > sides)
            throw new InvalidOperationException($"Scripted roll {roll} is impossible on a {sides}-sided die.");

        return roll;
    }
}
