using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("Touch Ended On 2D Object")]
[UnitCategory("Input")]
public class TouchEndedOnObjectUnit : Unit
{
    // Input: GameObject to check
    [DoNotSerialize]
    public ValueInput objectInput;

    // Output: Boolean indicating if touch ended on object
    [DoNotSerialize]
    public ValueOutput touchEnded;

    protected override void Definition()
    {
        objectInput = ValueInput<GameObject>("Object");
        touchEnded = ValueOutput<bool>("Touch Ended", EvaluateTouchEnded);
    }

    private bool EvaluateTouchEnded(Flow flow)
    {
        GameObject targetObject = flow.GetValue<GameObject>(objectInput);
        if (targetObject == null)
            return false;

        // Loop through all touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Ended)
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
                        return true; // Touch ended on the collider
                    }
                }
            }
        }

        return false; // No touch ended on the object
    }
}