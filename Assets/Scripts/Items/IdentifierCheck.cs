using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentifierCheck : MonoBehaviour
{
    private void Start()
    {
        //Find all the gameobjects with a WorldController script
        WorldController[] worldControllers = FindObjectsOfType<WorldController>();

        //Loop through all the worldControllers
        for (int i = 0; i < worldControllers.Length; i++)
        {
            //Loop through another to compare with, nested loo
            for (int j = i + 1; j < worldControllers.Length; j++)
            {
                //Check the Identifiers for a match
                if (worldControllers[i].Identifier == worldControllers[j].Identifier)
                {
                    //If there is a match
                    Debug.LogError("Match found between " + worldControllers[i].gameObject.name + " and " + worldControllers[j].gameObject.name + " with Identifier " + worldControllers[i].Identifier + ".");
                }
            }
        }



        //Find all the gameobjects with a WorldController script
        Identifiers[] identifiers = FindObjectsOfType<Identifiers>();

        //Loop through all the worldControllers
        for (int i = 0; i < identifiers.Length; i++)
        {
            //Loop through another to compare with, nested loo
            for (int j = i + 1; j < identifiers.Length; j++)
            {
                //Check the Identifiers for a match
                if (identifiers[i].Identifier == identifiers[j].Identifier)
                {
                    //If there is a match
                    Debug.LogError("Match found between " + identifiers[i].gameObject.name + " and " + identifiers[j].gameObject.name + " with Identifier " + identifiers[i].Identifier + ".");
                }
            }
        }
    }
}
