using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Attack : MonoBehaviour
{
    public GameObject[] AttackUI;
    //public int attackSelection = 0;

    //----------CONTROLLING THE UI----------
    //Change the selection
    public void changeSelection(int attackSelection)
    {
        for (int i = 0; i < AttackUI.Length; i++)
        {
            Image image = AttackUI[i].GetComponent<Image>();
            Color color = image.color;

            if (i == attackSelection)
            {
                //Set the selected image to full colour
                color.a = 1;
            }
            else
            {
                //Set the others to transparent
                color.a = 0;
            }

            image.color = color;
        }
    }

}
