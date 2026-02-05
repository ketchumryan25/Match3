using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("Touch Phases on 2D Object")]
[UnitCategory("Input")]
public class TouchPhasesOn2DObjectUnit : Unit
{
    // Input: GameObject to check
    [DoNotSerialize]
    public ValueInput objectInput;

    // Outputs: Booleans for each touch phase
    [DoNotSerialize]
    public ValueOutput touchBegan;
    [DoNotSerialize]
    public ValueOutput touchMoved;
    [DoNotSerialize]
    public ValueOutput touchStationary;
    [DoNotSerialize]
    public ValueOutput touchEnded;
    [DoNotSerialize]
    public ValueOutput touchCanceled;

    protected override void Definition()
    {
        objectInput = ValueInput<GameObject>("Object");

        touchBegan = ValueOutput<bool>("Began", CheckTouchPhase(TouchPhase.Began));
        touchMoved = ValueOutput<bool>("Moved", CheckTouchPhase(TouchPhase.Moved));
        touchStationary = ValueOutput<bool>("Stationary", CheckTouchPhase(TouchPhase.Stationary));
        touchEnded = ValueOutput<bool>("Ended", CheckTouchPhase(TouchPhase.Ended));
        touchCanceled = ValueOutput<bool>("Canceled", CheckTouchPhase(TouchPhase.Canceled));
    }

    private System.Func<Flow, bool> CheckTouchPhase(TouchPhase phase)
    {
        return (flow) =>
        {
            GameObject targetObject = flow.GetValue<GameObject>(objectInput);
            if (targetObject == null)
                return false;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == phase)
                {
                    // Convert touch position to world point
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);
                    Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);

                    // Check if the point overlaps with the target object's BoxCollider2D
                    BoxCollider2D collider = targetObject.GetComponent<BoxCollider2D>();
                    if (collider != null)
                    {
                        if (collider.OverlapPoint(worldPoint2D))
                        {
                            return true; // Touch on object with matching phase
                        }
                    }
                }
            }
            return false;
        };
    }
}