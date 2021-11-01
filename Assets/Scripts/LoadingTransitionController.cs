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
    private float startingVolume;
    private AsyncOperation asyncLoad;
    private bool startedLoad;
    private Action onComplete;
    #endregion
    #region properties
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startedLoad && MusicPlayer != null)
        {
            //start fading bgm
            MusicPlayer.Player.volume = startingVolume * (1 - asyncLoad.progress);

            //once fade has finished, finish loading scene
        }
    }
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
        startedLoad = true;
        if (MusicPlayer != null)
        {
            //start fading bgm
            startingVolume = MusicPlayer.Player.volume;

            //once fade has finished, finish loading scene
        }

    }
    #endregion
}
