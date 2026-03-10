using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadGame()
    {
        StartCoroutine(_LoadGame());

        IEnumerator _LoadGame()
        {
            AsyncOperation loadOperations = SceneManager.LoadSceneAsync("Game");
            while (!loadOperations!.isDone) yield return null;
            
            //GameObject playerObj = GameObject.Find("Player");
            //Debug.Log(playerObj.name);
        }
    }
}
