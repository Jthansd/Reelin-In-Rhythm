using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance {  get; private set; }


    private void Awake()
    {
        Instance = this;
    }
}
