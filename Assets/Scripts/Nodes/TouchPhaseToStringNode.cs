using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("Touch Phase to String")]
[UnitCategory("Input")]
public class TouchPhaseToStringNode : Unit
{
    // Input: TouchPhase
    [DoNotSerialize]
    public ValueInput touchPhaseInput;

    // Output: String representation of the phase
    [DoNotSerialize]
    public ValueOutput phaseStringOutput;

    protected override void Definition()
    {
        touchPhaseInput = ValueInput<TouchPhase>("Touch Phase");
        phaseStringOutput = ValueOutput<string>("Phase String", GetPhaseString);
    }

    private string GetPhaseString(Flow flow)
    {
        var phase = flow.GetValue<TouchPhase>(touchPhaseInput);
        return phase.ToString();
    }
}