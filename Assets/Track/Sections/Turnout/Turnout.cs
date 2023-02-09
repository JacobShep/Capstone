using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turnout : MonoBehaviour
{
    public int[] destinations = new int[4];//these need to be set before run time, indicate the index of the section which connects in that direction
    //format for destinations [up, right, down, left]
    private int direction;//indicates the direction the turnout is currently heading

    private void Awake()
    {
        for (int i = 0; i < destinations.Length; i++)//set direction to the first valid 
        {
            if (destinations[i] != -1)
                direction = i;
        }
    }
    private void OnMouseUp()//when this collider is clicked change the direction
    {
        Debug.Log("clicked");
        do//move it one over and repeat if not valid direction
        {
            if (direction < destinations.Length - 1)
                direction++;
            else
                direction = 0;
        } while (destinations[direction] == -1);

        string str="";
        switch (direction)
        {
            case 0: str = "up"; break;
            case 1: str = "right"; break;
            case 2: str = "down"; break;
            case 3: str = "left"; break;
        }
        Debug.Log("Now pointing " + str);
        //after it decides the correct direction it needs to update the sprite to look correct
    }

    public int getNext(){ return destinations[direction]; }
}
