using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover_Static : MonoBehaviour
{
    public GameObject Cam;

    public int roomLocation = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Cam.GetComponent<CameraController>().currentStaticCameraGameObjectPosition = roomLocation;
        }
    }
}
