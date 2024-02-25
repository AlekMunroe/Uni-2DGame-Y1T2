using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfSaved : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //If the identifier script exists
        if (this.GetComponent<Identifiers>() != null)
        {
            //Get the identifier name
            string identifierName = this.GetComponent<Identifiers>().IdentifierName;

            //Format the name for the playerpref
            string playerprefName = "Destroyed_Item_" + identifierName;

            //If the playerpref exists
            if(PlayerPrefs.GetInt(playerprefName) == 1)
            {
                //Destroy this
                Destroy(this.gameObject);
            }
        }
    }
}
