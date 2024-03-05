using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.Animations;
using TMPro;

public class CutsceneController_01 : MonoBehaviour
{
    [Header("Characters")]
    public GameObject controllablePlayer;

    public GameObject hunterOp;
    Animator hunterOpAnim;

    public GameObject hunterPrisoner;
    Animator hunterPrisonerAnim;

    public GameObject scientist;
    Animator scientistAnim;

    public GameObject security01;
    Animator security01Anim;

    public GameObject security02;
    Animator security02Anim;

    [Header("Cameras")]
    public GameObject cam01;
    public GameObject cam02;

    [Header("UI")]
    public GameObject fadeScreen;
    Animator fadeScreenAnim;

    public GameObject subtitlesUI;
    public GameObject characterImage;
    public TMP_Text characterNameText;
    public TMP_Text subtitleText;

    [Header("Debug")]
    public TMP_Text DebugRoomText;

    // Start is called before the first frame update
    void Start()
    {
        //Set the debug
#if UNITY_EDITOR || DEBUG
        DebugRoomText.gameObject.SetActive(true);
#endif

        //Set the animations
        hunterOpAnim = hunterOp.GetComponent<Animator>();
        hunterPrisonerAnim = hunterPrisoner.GetComponent<Animator>();
        scientistAnim = scientist.GetComponent<Animator>();
        security01Anim = security01.GetComponent<Animator>();
        security02Anim = security02.GetComponent<Animator>();
        fadeScreenAnim = fadeScreen.GetComponent<Animator>();

        //Set the first camera active
        cam01.SetActive(true);
        cam02.SetActive(false);

        //Set misc
        characterImage.SetActive(false);

        //Start the cutscene
        StartCoroutine(Cutscene());
    }

    IEnumerator Cutscene()
    {
        yield return new WaitForSeconds(0.1f);

        DebugRoomText.text = "Science Laboratory";

        //Set the scientist's animation - Working at the table
        scientistAnim.Play("IdleLeft");

        //Fade in
        fadeScreenAnim.Play("FadeIn");

        yield return new WaitForSeconds(4);

        //Narrator: "In the pursuit of power, governments often tread dangerous paths..."

        characterNameText.text = "Narrator";
        subtitleText.text = "In the pursuit of power, governments often tread dangerous paths...";
        subtitlesUI.SetActive(true);

        yield return new WaitForSeconds(5);

        subtitlesUI.SetActive(false);
        characterNameText.text = "";
        subtitleText.text = "";

        //Flashback - White fade out
        fadeScreenAnim.Play("FadeOutWhite");

        yield return new WaitForSeconds(0.6f);

        //Set camera to hunter infiltrating a secure facility
        cam02.SetActive(true);
        cam01.SetActive(false);

        controllablePlayer.SetActive(true);
        controllablePlayer.GetComponent<CharacterController>().enabled = false;

        yield return new WaitForSeconds(0.4f);

        //Flashback - White in out
        fadeScreenAnim.Play("FadeInWhite");

        yield return new WaitForSeconds(1);

        //One year earlier text

        characterNameText.text = "Narrator";
        subtitleText.text = "One year earlier...";
        subtitlesUI.SetActive(true);

        DebugRoomText.text = "Science Laboratory - Flashback";

        yield return new WaitForSeconds(4);

        subtitlesUI.SetActive(false);
        characterNameText.text = "";
        subtitleText.text = "";

        //Narrator: "Hunter Blackwood, a decorated special ops agent, was tasked with uncovering a classified government experiment."

        characterNameText.text = "Narrator";
        subtitleText.text = "Hunter Blackwood, a decorated special ops agent, was tasked with uncovering a classified government experiment.";
        subtitlesUI.SetActive(true);

        yield return new WaitForSeconds(6);

        subtitlesUI.SetActive(false);
        characterNameText.text = "";
        subtitleText.text = "";

        //Allow the player to move (Tutorial) To look for uncovering the government experiment

        controllablePlayer.GetComponent<CharacterController>().enabled = true;
    }

    IEnumerator Cutscene02()
    {
        yield return new WaitForSeconds(0.1f);

        //Wait until the player walks into the room with all the super soldiers where they are wreaking havoc

        //Narrator: "But what he found was beyond imagination; a failed experiment that unleashed unspeakable horrors."

        //The player must shoot all the enemies
    }

    IEnumerator Cutscene03()
    {
        yield return new WaitForSeconds(0.1f);

        //Wait until Hunter kills 10 zombies

        //Cut to the security who hear the gun shots and a question mark appears above both their heads indicating they heard the gun shots

        //Camera - Follow the security guards who walk into the room with Hunter inside

        //One of the security guards shoot Hunter and they both walk onto him to show they have captured him

        //Narrator: "His loyalty was repaid with betrayal, as he was branded a traitor by those he once served."

        //While the narrator is speaking, the security guards pull Hunter away

        //Fade out and in

        //There are prison guards walking Hunter in a high-security prison

        //Narrator: "Now imprisoned and labeled a traitor, Hunter Blackwood vows to clear his name and expose the corruption that led to his wrongful incarceration."

        //Zoom into Hunter while fading out
    }
}
