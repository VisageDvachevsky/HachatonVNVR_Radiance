using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnToLobby : MonoBehaviour
{
    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void LoadLobbyAfterDelay(float delay)
    {
        StartCoroutine(LoadSceneWithDelay(delay));
    }

    private IEnumerator LoadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Lobby");
    }
}
