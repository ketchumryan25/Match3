using Unity.VisualScripting;

[UnitTitle("Bool Trigger")]
[UnitCategory("Custom")]
public class BoolTriggerUnit : Unit
{
    // Input port for boolean value
    [DoNotSerialize]
    public ValueInput inputBool;

    // Control input port to trigger the evaluation
    [DoNotSerialize]
    public ControlInput trigger;

    // Control output ports for true and false branches
    [DoNotSerialize]
    public ControlOutput onTrue;
    [DoNotSerialize]
    public ControlOutput onFalse;

    protected override void Definition()
    {
        // Define the boolean input
        inputBool = ValueInput<bool>("Bool", false);

        // Define the control input trigger
        trigger = ControlInput("Trigger", Enter);

        // Define the control outputs
        onTrue = ControlOutput("True");
        onFalse = ControlOutput("False");

        // Set up the flow: when trigger is activated, evaluate
        Succession(trigger, onTrue);
        Succession(trigger, onFalse);
    }

    // Called when the trigger port is activated
    private ControlOutput Enter(Flow flow)
    {
        bool value = flow.GetValue<bool>(inputBool);

        if (value)
        {
            flow.Invoke(onTrue);
        }
        else
        {
            flow.Invoke(onFalse);
        }

        // Return null since flow is invoked manually
        return null;
    }
}