using UnityEngine;

public class WaterController : MonoBehaviour
{

    public static event System.Action<GameObject> OnBobberEnteredWater;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bobber"))
        {
            Debug.Log("Bobber entered the water!");
            OnBobberEnteredWater?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bobber"))
        {
            Debug.Log("Bobber left the water!");
        }
    }
}
