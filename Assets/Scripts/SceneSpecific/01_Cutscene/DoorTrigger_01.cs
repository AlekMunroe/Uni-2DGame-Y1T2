using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using TMPro;
using UnityEngine.Playables;

public class DoorTrigger_01 : MonoBehaviour
{
    public GameObject cam02;
    public GameObject cam03;
    public GameObject doorCollider;

    public CutsceneController_01 cutsceneController_01;

    public GameObject tutorialUIImage;
    public TMP_Text tutorialTitleText;
    public TMP_Text tutorialDescriptionText;
    Animation tutorialUIAnim;

    public PlayableDirector director;
    public GameObject pauseDirector;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //cam03.SetActive(true);
            //cam02.SetActive(false);

            //doorCollider.SetActive(true);

            //StartCoroutine(cutsceneController_01.Cutscene02());

            pauseDirector.SetActive(false);
            director.Play();
        }
    }

    IEnumerator TutorialUI()
    {
        tutorialTitleText.text = "Goal";
        tutorialDescriptionText.text = "Set the super soldiers free.";

        tutorialUIAnim.Play("TutorialUIAnim");

        yield return new WaitForSeconds(4);
    }
}
