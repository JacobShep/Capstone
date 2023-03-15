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
        Debug.Log("Next section: " + next);
        Section nextSection = sections[next];
        Section lastSection = sections[last];

        int ep;
        if (lastSection.isReversed())
            ep = 0;
        else
            ep = 1;

        bool passingThrough = false;
        if (lastSection.myEndpoints[ep] is Passthrough)
            passingThrough = true;

        if (!nextSection.isLocked())//this is the start of a critical region when working with race conditions this will need to be locked
        {//it isnt locked
            Debug.Log("Changing sections!");

            #region Next Section Reverse or Not

            Debug.Log("Orientation of section " + nextSection.order + " is " + nextSection.getOrientationActual() + " turnout is facing " + lastSection.myEndpoints[ep].getDirection());
            if (nextSection.getOrientationActual() == lastSection.myEndpoints[ep].getDirection())
                nextSection.Reverse(false);
            else
                nextSection.Reverse(true);
            #endregion

            #region Restart Train
            if (t.isStopped())//direction you are entering from
            {
                //Debug.Log("Train " + t.getID() + "is starting again");
                t.restart(-1f);//restart the train at default speed
            }
            #endregion

            #region Locking inaccessable turnouts
            if (lastSection.isReversed())
                turnoutAdjust(last, 1);//it entered at 1 and exited at 0 so 1 might close
            else
                turnoutAdjust(last, 0);
            #endregion

            lastSection.Exit();//we are protected by a mutex so it shouldnt matter that we exit first
            nextSection.Enter();
            
            t.setIndex(next);
        }
        else if (!t.isStopped())//next is locked hault the train
        { 
            if (passingThrough && (lastSection.myEndpoints[ep].getNext() == lastSection.order))//if its pointing to itself
                ((Passthrough)(lastSection.myEndpoints[ep])).swapDirections();//try letting the train through

            t.hault();
        }
    }

    private void turnoutAdjust(int sect, int toClose) //alter this function so it only checks the one you didnt just use ie the far one since the closer will be within the section you enter
    {
        bool closeit = true;
        int[] arr = sections[sect].myEndpoints[toClose].getSections();
        foreach (int i in arr)
        {
            if (i != -1 && sections[i].isLocked() && i != sect)
            { Debug.Log("section " + i + " is still using this turnout " + toClose); closeit = false; break; }//one of the other sections needs this turnout active
        }
        if (closeit)
            sections[sect].myEndpoints[toClose].deactivate();
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
