using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public SceneField nextScene;

    public void LoadNextScene()
    {
        LoadScene(nextScene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void LoadScene(SceneField scene)
    {
        StartCoroutine(LoadingScene(scene));
    }

    private IEnumerator LoadingScene(SceneField scene)
    {
        Debug.Log("loading " + scene);
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(scene);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
    }
}