using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoSingleton<SceneHandler>
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

public struct Scenes
{
    public const string MainMenu = "MainMenu";
    public const string Game = "Game";
}