using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public Path myP;
    public Signal mySig;
    private byte locked = 0;
    private int id;
    public int order;


    void Awake()
    {
        id = (int)Random.Range(1f, 100f);
        if (myP is null)
            Debug.Log("Null path");
        if (mySig is null)
            Debug.Log("Null signal");
        //Debug.Log("My path:" + myP.getPath());
        //if (myP is null)
        //    Debug.Log("path is null");
        //Debug.Log("Section ID: " + id);
    }

    public Transform Next()
    {
        Transform t = myP.Next();
        if (t is null)
            return null;
        else
            return t;
    }

    public Transform getCur()
    {
        return myP.getCur();
    }

    public void Enter()
    {
        Debug.Log("Just locked section: " + id);
        locked = 1;
        mySig.close();
        //also need to change signal sprites here
    }

    public void Exit()
    {
        Debug.Log("Just unlocked section: " + id);
        locked = 0;
        mySig.open();
        //also need to change signal sprites here
    }

    public bool isLocked()
    {
        if (locked == 1)
            return true;
        else
            return false;
    }

    public string getPath()
    {
        return myP.getPath();
    }

    public int getID(){ return id; }
}
