using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("Touch Ended Raycast Hit Object")]
[UnitCategory("Input")]
public class TouchEndedRaycastHitObjectUnit : Unit
{
    // Optional: Filter for specific GameObject
    [DoNotSerialize]
    public ValueInput objectFilter;

    // Output: The GameObject hit during touch ended
    [DoNotSerialize]
    public ValueOutput hitObject;

    protected override void Definition()
    {
        objectFilter = ValueInput<GameObject>("Object (Optional)", null);
        hitObject = ValueOutput<GameObject>("Hit Object", GetHitObject);
    }

    private GameObject GetHitObject(Flow flow)
    {
        GameObject filterObject = flow.GetValue<GameObject>(objectFilter);
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Ended)
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);
                Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);

                RaycastHit2D hit2D = Physics2D.Raycast(worldPoint2D, Vector2.zero);
                if (hit2D.collider != null)
                {
                    if (filterObject == null || hit2D.collider.gameObject == filterObject)
                    {
                        return hit2D.collider.gameObject;
                    }
                }
            }
        }
        return null;
    }
}