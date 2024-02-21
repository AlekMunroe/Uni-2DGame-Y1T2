using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Generic")]
    public GameObject player;

    [Header("Camera information")]
    public bool followPlayer;
    public bool staticCamera;

    [Header("Movement")]
    //Static camera specific
    public GameObject[] staticCameraPosition;
    public int currentStaticCameraGameObjectPosition;
    public float speed = 5;
    private bool isMoving = false;
    bool usedSavePosition;

    //Used by both camera types
    Vector3 currentPosition;
    Vector3 newPosition;



    // Start is called before the first frame update
    void Start()
    {
        //Error checking
        if(!followPlayer && !staticCamera)
        {
            Debug.LogError("You must enable followPlayer or staticCamera.");
        }
        else if(followPlayer && staticCamera)
        {
            Debug.LogError("You must enable either followPlayer or staticCamera. Not both.");
        }

        //Setting the position
        //Initialise the camera position from saved preferences
        InitialisePosition();

        //If the camera does not do anything in InitialisePosition
        if (staticCamera && !usedSavePosition)
        {
            Debug.Log("2");
            currentPosition = new Vector3(staticCameraPosition[0].transform.position.x, staticCameraPosition[0].transform.position.y, -10);
            this.transform.position = currentPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            FollowPlayer();
        }
        else if (staticCamera)
        {
            StaticCamera();
        }
    }

    void FollowPlayer()
    {
        //Get the current position of the camera
        currentPosition = this.transform.position;

        //Get the next position of the camera
        newPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10);

        //Actually move the camera
        this.transform.position = newPosition;
    }

    void StaticCamera()
    {
        newPosition = new Vector3(staticCameraPosition[currentStaticCameraGameObjectPosition].transform.position.x, staticCameraPosition[currentStaticCameraGameObjectPosition].transform.position.y, -10);

        //Move the camera
        transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);

        //If the camera is close enought to the target position
        if (Vector3.Distance(transform.position, newPosition) < 0.01f)
        {
            transform.position = newPosition; //Snap the camera in place of the target position (Making sure the camera is exact)
        }
    }

    void InitialisePosition()
    {
        if (PlayerPrefs.HasKey("Camera_PositionX") && PlayerPrefs.HasKey("Camera_PositionY")) //If the positions exist (A save state has been set)
        {
            Debug.Log("1");
            usedSavePosition = true;

            //Get the saved positions
            float savedX = PlayerPrefs.GetFloat("Camera_PositionX");
            float savedY = PlayerPrefs.GetFloat("Camera_PositionY");
            currentStaticCameraGameObjectPosition = PlayerPrefs.GetInt("Camera_CameraNextLocation");

            //Set the camera to the saved positions
            currentPosition = new Vector3(savedX, savedY, -10);
            this.transform.position = currentPosition;
        }
        else
        {
            currentPosition = Vector3.zero; //Default to zero if no saved position
        }
    }
}
