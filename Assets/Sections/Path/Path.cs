using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Transform[] waypoints;
    private int index;
    void Awake()
    {
        index = 0;

        waypoints = new Transform[transform.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }

        //string msg = getPath();
        //Debug.Log("This Path is : " + msg);
    }

    public Transform getCur()
    {
        return waypoints[index];
    }

    public Transform Next()
    {
        if (index < waypoints.Length - 1)
        {
            index++;
            return waypoints[index];//give next waypoint
        }
        else
        {
            index = 0;//reset this path
            return null;//we are at the end go to next section
        }
    }

    public string getPath()
    {
        string msg = "";
        foreach (Transform t in waypoints)
        {
            msg += t.position;
            msg += " ";
        }
        return msg;
    }
}
