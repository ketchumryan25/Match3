using UnityEngine;
using Unity.VisualScripting;

[UnitCategory("Events")]
[UnitTitle("On Integer Value Changed")]
public class OnIntValueChangedUnit : Unit
{
    // Input port for the current value
    [DoNotSerialize]
    public ValueInput valueInput;

    // Control port to trigger the check
    [DoNotSerialize]
    public ControlInput triggerInput;

    // Output port for event
    [DoNotSerialize]
    public ControlOutput onChangedOutput;

    // Internal storage for previous value
    private int previousValue;

    protected override void Definition()
    {
        // Define input port for value
        valueInput = ValueInput<int>("Value");
        // Define control input port
        triggerInput = ControlInput("Trigger", (flow) => {
            CheckForChange(flow);
            return onChangedOutput;
        });
        // Define control output port
        onChangedOutput = ControlOutput("OnChanged");

        // Establish the flow: when Trigger is called, run CheckForChange, then go to OnChanged
        Succession(triggerInput, onChangedOutput);
    }

    private void CheckForChange(Flow flow)
    {
        int currentVal = flow.GetValue<int>(valueInput);
        if (currentVal != previousValue)
        {
            previousValue = currentVal;
            flow.Invoke(onChangedOutput);
        }
    }
}