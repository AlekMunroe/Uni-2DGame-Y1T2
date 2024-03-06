using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations;

public class Tutorial_Key : MonoBehaviour
{
    public GameObject tutorialUIImage;
    public TMP_Text tutorialTitleText;
    public TMP_Text tutorialDescriptionText;
    Animation tutorialUIAnim;

    public GameObject key;

    private void Start()
    {
        tutorialUIAnim = tutorialUIImage.GetComponent<Animation>();

        StartCoroutine("Run");
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(40); //Allowing 40 seconds from the start of the scene before showing the tutorial UI prompt

        if (key.gameObject.activeInHierarchy)
        {
            tutorialTitleText.text = "Collect items";
            tutorialDescriptionText.text = "To collect items, walk up to them.";

            tutorialUIAnim.Play("TutorialUIAnim");

            yield return new WaitForSeconds(4);

            tutorialTitleText.text = "Collect items";
            tutorialDescriptionText.text = "Pick up the key from the desk to open the door.";
        }
    }
}
