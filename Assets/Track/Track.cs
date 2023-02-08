using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    // Start is called before the first frame update

    private Train[] trains;
    private Section[] sections;
    public Transform s;
    private Vector3 spawn;
    public GameObject prefab;
    void Awake()
    {
        trains = FindObjectsOfType<Train>();
        //spawn = s.position;
        //spawnTrain();
        sections = FindObjectsOfType<Section>();

        //sorting
        Section[] temp = new Section[sections.Length];
        foreach (Section s in sections)
        {
            temp[s.order] = s;
        }
        sections = temp;

        //Debug.Log("number of sections found: " + sections.Length);
        //foreach (Section s in sections)
        //{
        //    Debug.Log("This sections id is: "+s.getID()+"\n"+"This sections path:"+s.getPath());
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
            spawnTrain();

        foreach (Train t in trains)
        {
            int i = t.getIndex();
            if (t.isReady())//check if we need to get next
            {//getting next
                t.resetReady();
                Transform trans = sections[i].Next();
                if (trans is null)//at end of path
                {
                    i++;
                    //Debug.Log("Train "+t.getID()+" is trying to leaving section "+i);
                    if (i < sections.Length)//check for end of array 
                        changeSections(t, i); //not the last section move on
                    else//is the last section loop back
                        changeSections(t, 0);
                }
                else//Do not need to leave yet
                    t.Move(trans);
            }
            else
                t.Move(sections[i].getCur());//keep going at cur
        }
    }

    private void changeSections(Train t, int i)
    {
        int last;

        if (i == 0)
            last = sections.Length - 1;
        else
            last = i - 1;

        if (!sections[i].isLocked())//this is the start of a critical region when working with race conditions this will need to be locked
        {//it isnt locked
            if (t.isStopped())//if it had to stop before
            {
                Debug.Log("Train " + t.getID() + "is starting again");
                t.restart(-1f);
            }
            sections[i].Enter();//enter first so that we dont get stuck in nowhere
            sections[last].Exit();
            t.setIndex(i);
        }
        else if (!t.isStopped())//should only be called the first time
        { //next is locked
            Debug.Log("Section is locked stopping train "+t.getID());
            t.hault();
        }
    }

    private void spawnTrain()
    {
        if (sections[0].isLocked())
        {
            Debug.Log("Cant spawn a train while section is ocupied");
            return;
        }
        //create a train prefab at spawn 
        //tell that train to target section with first tag

        Instantiate(prefab, s);//spawn train at s
        trains = FindObjectsOfType<Train>();//Gotta grab the new object...not very efficient look for a different method later
        sections[0].Enter();
    }
}
