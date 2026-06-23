using UnityEngine;

public class ReelWheelNote : MonoBehaviour
{
    public float degreesToTravel;
    private float spawnAngle;

    void Start()
    {
        spawnAngle = transform.parent.localEulerAngles.z;
    }

    void Update()
    {
        float currentAngle = transform.parent.localEulerAngles.z;

        // Clockwise travel (positive as wheel rotates forward)
        float traveledCW = Mathf.DeltaAngle(currentAngle, spawnAngle);

        if (traveledCW >= degreesToTravel)
        {
            Destroy(gameObject);
        }
    }
}
