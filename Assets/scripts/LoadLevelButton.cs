using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    [SerializeField] string _LevelName;

    public void Load()
    {
        SceneManager.LoadScene(_LevelName);
    }
}