using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Waypoint[] waypoints;
    private int index;
    void Awake()
    {
        index = 0;

        waypoints = GetComponentsInChildren<Waypoint>();
        Waypoint[] temp = waypoints;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            for (int j = 0; j < waypoints.Length - i - 1; j++)
            {
                if (waypoints[j].id > waypoints[j + 1].id)
                {
                    Waypoint w = waypoints[j];
                    waypoints[j] = waypoints[j + 1];
                    waypoints[j + 1] = w;
                }
            }
        }
        //string msg = getPath();
        //Debug.Log("This Path is : " + msg);
    }

    public Waypoint getCur()
    {
        return waypoints[index];
    }

    public Waypoint NextF() //gives the path from 0->n
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

    public Waypoint NextR() //n->0
    {
        if (index > 0)
        {
            index--;
            return waypoints[index];//give next waypoint
        }
        else
        {
            index = 0;//reset this path
            return null;//we are at the end go to next section
        }
    }

    public void PrepReverse()//gets ready to go backwards
    {
        index = waypoints.Length - 1;
    }
    public string getPath()
    {
        string msg = "";
        foreach (Waypoint w in waypoints)
        {
            msg += w.transform.position;
            msg += " ";
        }
        return msg;
    }

    public Waypoint GetWaypointByID(int _id)
    {
        foreach (Waypoint w in waypoints)
            if (w.id == _id)
                return w;
        return null;
    }

    public void SetIndex(int _index)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i].id == _index)
            { 
                index = i;
                return;
            }

        }
    }
}
