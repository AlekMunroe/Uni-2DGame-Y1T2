using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations;

public class Tutorial_Movement : MonoBehaviour
{
    public GameObject tutorialUIImage;
    public TMP_Text tutorialTitleText;
    public TMP_Text tutorialDescriptionText;
    Animation tutorialUIAnim;

    bool hasMoved;

    private void Start()
    {
        tutorialUIAnim = tutorialUIImage.GetComponent<Animation>();

        StartCoroutine("Run");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            //Disable the tutorial UI if you have already moved
            hasMoved = true;
        }
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(25); //Allowing 25 seconds from the start of the scene before showing the tutorial UI prompt

        if (hasMoved == false)
        {
            tutorialTitleText.text = "Move";
            tutorialDescriptionText.text = "Use WASD to move.";

            tutorialUIAnim.Play("TutorialUIAnim");
        }
    }
}
