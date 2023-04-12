using UnityEngine;
public class Section : MonoBehaviour
{
    public Path myP;
    public Signal[] mySig;
    public Endpoint[] myEndpoints;
    public string orientationInput;
    private int orientationActual;
    private byte locked = 0;//0 is open 1 is closed
    private int id;
    public int order;
    private bool isReverse;

    void Awake()
    {
        switch (orientationInput) 
        {
            case "up": orientationActual = IDirections.UP; break;
            case "right": orientationActual = IDirections.RIGHT; break;
            case "down": orientationActual = IDirections.DOWN; break;
            case "left": orientationActual = IDirections.LEFT; break;
        }

        id = (int)Random.Range(1f, 100f);
        if (myP is null)
            Debug.Log("Null path");
        if (mySig is null)
            Debug.Log("Null signal");
        isReverse = false;
        //Debug.Log("My path:" + myP.getPath());
        //if (myP is null)
        //    Debug.Log("path is null");
        //Debug.Log("Section ID: " + id);
    }

    public Waypoint Next()
    {
        Waypoint w; 
        if (isReverse)
            w = myP.NextR();
        else
            w = myP.NextF();

        return w;
    }

    public int getNextSection() 
    {
        int nextSection;
        if (isReverse)
        {
            nextSection = myEndpoints[0].getNext();
            return nextSection;
        }
        else
        {
            nextSection = myEndpoints[1].getNext();
            return nextSection;
        }
    }

    public Waypoint getCur()
    {
        return myP.getCur();
    }

    public void Enter()
    {
        locked = 1;
        foreach (Signal s in mySig)
            s.close();
        myEndpoints[0].activate();
        myEndpoints[1].activate();
    }

    public void Exit()
    {
        //Debug.Log("Just unlocked section: " + id);
        locked = 0;
        isReverse = false;
        foreach (Signal s in mySig)
            s.open();
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
    public void Reverse(bool r) { isReverse = r; if(r) myP.PrepReverse(); }
    public bool isReversed() { return isReverse; }
    public int getOrientationActual() { return orientationActual; }
    public void Lock() { locked = 1; }
    public void Unlock() { locked = 0; }
}
