using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Stop Trigger When True")]
[UnitCategory("Custom")]
public class StopTriggerWhenTrueUnit : Unit
{
    // Control ports
    [DoNotSerialize]
    public ControlInput triggerInput;

    [DoNotSerialize]
    public ControlOutput triggerOutput;

    // Boolean input
    [DoNotSerialize]
    public ValueInput stopCondition;

    protected override void Definition()
    {
        // Define port for trigger input
        triggerInput = ControlInput("Trigger", (flow) =>
        {
            bool shouldStop = flow.GetValue<bool>(stopCondition);
            if (shouldStop)
            {
                // If condition is true, do not continue flow
                // Optionally, you could add logging or other behavior here
                return null; // Stops the flow
            }
            else
            {
                // Continue flow if false
                return triggerOutput;
            }
        });

        // Define port for trigger output
        triggerOutput = ControlOutput("Done");

        // Define boolean input
        stopCondition = ValueInput<bool>("Stop When True", false);

        // Connect control flow
        Succession(triggerInput, triggerOutput);
    }
}