using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PauseDirector : MonoBehaviour
{
    public bool disableDirector;
    public bool disablePlayerController;

    public bool enablePlayerController;

    public PlayableDirector director;
    public CharacterController playerController;

    private void OnEnable()
    {
        if (disableDirector)
        {
            director.Pause();
            director.enabled = false;
        }
        else if (disablePlayerController)
        {
            playerController.FreezeControls();

            playerController.enabled = false;
        }
        else if (enablePlayerController)
        {
            playerController.UnfreezeControls();

            playerController.enabled = true;
        }

        this.gameObject.SetActive(false);
    }
}
