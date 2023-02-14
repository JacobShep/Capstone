using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    // Start is called before the first frame update

    private Train[] trains;
    private Section[] sections;
    public Transform s;
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
                //Debug.Log("Reached index: " + sections[i].myP.getCur() + " in section: " + i);
                t.resetReady();
                Transform trans = sections[i].Next();
                if (trans is null)//at end of path
                {
                    changeSections(t, sections[i].getNextSection(), i);//train, where we are going, where we are/were
                }
                else//Do not need to leave yet
                    t.Move(trans);
            }
            else
                t.Move(sections[i].getCur());//keep going at cur
        }
    }

    private void changeSections(Train t, int next, int last)
    {

        if (!sections[next].isLocked())//this is the start of a critical region when working with race conditions this will need to be locked
        {//it isnt locked
            #region Next Section Reverse or Not
            Debug.Log("Next: " + next + " Last: " + last);
            if (next == 0 && last == sections.Length - 1)
                sections[next].Reverse(false);
            else if (last > next || (last == 0 && next == sections.Length - 1))
            { 
                sections[next].Reverse(true);
                sections[next].myP.PrepReverse();
            }
            else
                sections[next].Reverse(false);
            #endregion

            if (t.isStopped())//direction you are entering from
            {
                //Debug.Log("Train " + t.getID() + "is starting again");
                t.restart(-1f);//restart the train at default speed
            }

            if (sections[last].isReversed())
                turnoutAdjust(last, 1);//it entered at 1 and exited at 0 so 1 might close
            else
                turnoutAdjust(last, 0);

            sections[next].Enter();//enter first so that we dont get stuck in nowhere
            sections[last].Exit();

            t.setIndex(next);
        }
        else if (!t.isStopped())//should only be called the first time
        { //next is locked
            //Debug.Log("Section is locked stopping train "+t.getID());
            t.hault();
        }
    }

    private void turnoutAdjust(int sect, int toClose) //alter this function so it only checks the one you didnt just use ie the far one since the closer will be within the section you enter
    {
        bool closeit = true;
        int[] arr = sections[sect].myTurn[toClose].getSections();
        foreach (int i in arr)
        {
            if (i != -1 && sections[i].isLocked() && i != sect)
            { Debug.Log("section " + i + " is still using this turnout " + toClose); closeit = false; break; }//one of the other sections needs this turnout active
        }
        if (closeit)
            sections[sect].myTurn[toClose].deactivate();
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
