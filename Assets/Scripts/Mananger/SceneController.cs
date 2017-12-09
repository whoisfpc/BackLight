using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public SceneField mainScene;
    public SceneField level1Scene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void BackToMenu()
    {
        LoadScene(mainScene);
    }

    public void StartGame()
    {
        LoadScene(level1Scene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene(SceneField scene)
    {
        StartCoroutine(LoadingScene(scene));
    }

    IEnumerator LoadingScene(SceneField scene)
    {
        Debug.Log("loading " + scene);
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(scene);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
    }
}