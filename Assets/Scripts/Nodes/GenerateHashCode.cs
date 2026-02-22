using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("Generate Hash Code")]
[UnitCategory("Custom")]
public class GenerateHashCode : Unit
{
    // Output: UUID as string
    [DoNotSerialize]
    public ValueOutput hashOutput;

    protected override void Definition()
    {
        hashOutput = ValueOutput<string>("HashCode", getRandomSeed);
    }

    string getRandomSeed(Flow flow)
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        return seed;
    }
}
