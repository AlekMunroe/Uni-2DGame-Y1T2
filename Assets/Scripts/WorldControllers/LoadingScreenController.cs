using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI")]
    public Image loadingBar;
    public TMP_Text loadingPercentage;

    public GameObject loadingScreen;

    [Header("Options")]
    public bool showLoadingBar = false;

    //Used to control from another script
    public void loadScene(int sceneToLoad)
    {
        StartCoroutine(LoadScene(sceneToLoad));
    }

    IEnumerator LoadScene(int sceneToLoad)
    {
        loadingScreen.SetActive(true);

        AsyncOperation loadLevel = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!loadLevel.isDone)
        {
            //Loading bar (opitonal)
            if (showLoadingBar)
            {
                loadingBar.fillAmount = Mathf.Clamp01(loadLevel.progress / 0.9f);
            }

            float percentage = loadLevel.progress * 100f; //Calculate percentage

            //Loading percentage
            Debug.Log("Loading Percentage: " + percentage.ToString("F2") + "%");

            loadingPercentage.text = "Loading... " + percentage.ToString("F2") + "%";

            yield return null;
        }
    }
}
