using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Train : MonoBehaviour
{
    public float rotationSpeed;//to do rotation I can rotate when exiting section and exit direction != current rotation using similar code to turnouts
    public int Speed { get; private set; }
    private int defSpeed = 1;
    private int sectionIndex = 0;//defaults to section 0
    private bool end, stopped = false;
    public TextMeshPro Username { get; set; }
    public int ID { get; set; }
    private Waypoint currentTarget;

    //Move is called by Track every tick

    private void Awake()
    {
        Speed = defSpeed;
        ID = (int)UnityEngine.Random.Range(1f, 1000f);//going to need to adjust this so that every id is unique
        Username = GetComponentInChildren<TextMeshPro>();
    }
    public void Move(Waypoint target)//need to do something about waiting in here
    {
        currentTarget = target;
        Transform trans = target.transform;
        try
        {
            if (Vector3.Distance(transform.position, trans.position) <= 0.2f || stopped)//if we are pretty close to the target we need a new one 
            {
                end = true;
                return;
            }

            Vector3 dir = trans.position - transform.position;//compare location to target
            transform.Translate(dir.normalized * Speed * Time.deltaTime);//move
        }
        catch (Exception)
        {
            Debug.Log("Failed to move train " + ID);
        }
    }

    public void hault() 
    {
        Speed = 0;
        stopped = true;
    }

    public void restart(int s)//use -1 to reset to default speed
    {
        switch (s)
        {
            case -1: Speed = defSpeed; break;
            default: Speed = s; break;
        }

        stopped = false;
    }

    public void SpeedChange(int s)//use -1 to reset to default speed
    {
        switch (s)
        {
            case -1: Speed = defSpeed; break;
            default: Speed = s; break;
        }
    }

    public string getLocation() 
    {
        Debug.LogError("Sending the target ID as:" + currentTarget.id);
        return ID + INetwork_Utils.DELIM + currentTarget.id +INetwork_Utils.DELIM + sectionIndex + INetwork_Utils.DELIM + transform.position.ToString(); 
    }
    public int GetSectionIndex() { return sectionIndex; }
    public void SetSectionIndex(int ind) { sectionIndex = ind; }

    public bool isReady() { return end; }

    public void resetReady() { end = false; }

    public bool isStopped() { return stopped; }

    public void SetTarget(Waypoint target)
    {
        currentTarget = target;
    }
}
