using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public float rotationSpeed;
    private float speed;
    private float defSpeed = 2f;//setting speed onAwake made the trains to fast when instantiating on the fly so set them both to the same and let speed change over time
    private int index = 0;//defaults to section 0
    private bool end, stopped = false;
    private int id;//identifies the train for debugging

    //Move is called by Track every tick

    private void Awake()
    {
        speed = defSpeed;
        id = (int)Random.Range(1f, 100f);//just needed for debugging
    }
    public void Move(Transform target)//need to do something about waiting in here
    {
        if (Vector3.Distance(transform.position, target.position) <= 0.2f || stopped)//if we are pretty close to the target we need a new one 
        {
            end = true;
            return;
        }

        Vector3 dir = target.position - transform.position;//compare location to target
        transform.Translate(dir.normalized * speed * Time.deltaTime);//move
    }

    public void hault() 
    {
        speed = 0f;
        stopped = true;
    }

    public void restart(float s)//use -1 to reset to default speed
    {
        switch (s)
        {
            case -1: speed = defSpeed; break;
            default: speed = s; break;
        }

        stopped = false;
    }
    public int getIndex() { return index; }
    public void setIndex(int ind) { index = ind; }

    public bool isReady() { return end; }

    public void resetReady() { end = false; }

    public bool isStopped() { return stopped; }

    public int getID() { return id; }
}
