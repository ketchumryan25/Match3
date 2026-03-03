using UnityEngine;
using Unity.VisualScripting;

[UnitCategory("Custom")]
[UnitTitle("Is Null")]
public class IsNullUnit : Unit
{
    [DoNotSerialize]
    public ValueInput input;

    [DoNotSerialize]
    public ValueOutput output;

    protected override void Definition()
    {
        input = ValueInput<object>("Input");
        output = ValueOutput<bool>("IsNull", IsNull);
        Requirement(input, output);
    }

    private bool IsNull(Flow flow)
    {
        var value = flow.GetValue<object>(input);
        return value == null;
    }
}