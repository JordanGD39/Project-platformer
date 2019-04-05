using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScript : MonoBehaviour
{
    public Texture heart;
    public int amountOfHearts = 3;
    public int newLine = 0;
    public int health = 3;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        checkHealth();

        if (Input.GetKeyDown(KeyCode.R))
        {
            health -= 1;
           // if (currhealth <= 5)
            //{
               // newLine = 0;
               // amountOfHearts -= 1;
            //}
            //else
            //{
                //newLine -= 1;
            //}
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            health += 1;
            //if (health <= 4)
            //{
            //amountOfHearts += 1;
            //
            //else
            //{
            //newLine += 1;
            //}
            //if (health >= 10)
            //{
            //newLine = 5;
            //}
        }
    }

    void checkHealth()
    {
        switch (health)
        {
            case 0:
                amountOfHearts = 0;
                break;
            case 1:
                amountOfHearts = 1;
                break;
            case 2:
                amountOfHearts = 2;
                break;
            case 3:
                amountOfHearts = 3;
                break;
            case 4:
                amountOfHearts = 4;
                break;
            case 5:
                amountOfHearts = 5;
                break;
            case 6:
                amountOfHearts = 5;
                newLine = 1;
                break;
            case 7:
                amountOfHearts = 5;
                newLine = 2;
                break;
            case 8:
                amountOfHearts = 5;
                newLine = 3;
                break;
            case 9:
                amountOfHearts = 5;
                newLine = 4;
                break;
            case 10:
                amountOfHearts = 5;
                newLine = 5;
                break;
        }
    }

    private void OnGUI()
    {
        if (heart)
        {
            for (int i = 0; i < amountOfHearts; i++)
            {
                GUI.DrawTexture(new Rect(10 + (45 * i), 10, 40, 60), heart, ScaleMode.ScaleToFit, true);
            }

            if (health >= 6)
            {
                for (int i = 0; i < newLine; i++)
                {
                    GUI.DrawTexture(new Rect(10 + (45 * i), 47, 40, 60), heart, ScaleMode.ScaleToFit, true);
                }
            }
        }
    }
}
