using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("Get Touch Phase")]
[UnitCategory("Input")]
public class GetTouchPhaseUnit : Unit
{
    // Optional: index of the touch (default 0)
    [DoNotSerialize]
    public ValueInput touchIndex;

    // Output port for the touch phase as a string
    [DoNotSerialize]
    public ValueOutput phase;

    protected override void Definition()
    {
        // Define the input for touch index
        touchIndex = ValueInput<int>("Touch Index", 0);

        // Define the output for the touch phase
        phase = ValueOutput<string>("Touch Phase", GetTouchPhase);
    }

    private string GetTouchPhase(Flow flow)
    {
        int index = flow.GetValue<int>(touchIndex);
        if (Input.touchCount > index)
        {
            Touch touch = Input.GetTouch(index);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    return "Began";
                case TouchPhase.Moved:
                    return "Moved";
                case TouchPhase.Stationary:
                    return "Stationary";
                case TouchPhase.Ended:
                    return "Ended";
                case TouchPhase.Canceled:
                    return "Canceled";
                default:
                    return "Unknown";
            }
        }
        else
        {
            return "No Touch";
        }
    }
}