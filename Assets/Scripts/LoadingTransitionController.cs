using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingTransitionController : MonoBehaviour
{
    #region constants
    #endregion
    #region public fields
    public MusicPlayerController MusicPlayer;
    #endregion
    #region private fields
    private AsyncOperation asyncLoad;
    private Action onComplete;
    #endregion
    #region properties
    #endregion
    #region unity methods

    #endregion
    #region actions
    public void ActionFinishLoading(AsyncOperation op)
    {
        onComplete?.Invoke();
    }
    #endregion
    #region public methods
    public void LoadScene(string sceneName, Action onComplete)
    {
        //show screen
        gameObject.SetActive(true);
        //start loading new scene
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.completed += ActionFinishLoading;
        asyncLoad.allowSceneActivation = true;

        this.onComplete = onComplete;
        if (MusicPlayer != null)
        {
            MusicPlayer.FadeVolume(0, 0.5f);
        }

    }
    #endregion
}
