using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance {  get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }
}
