using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
public class Track : MonoBehaviour
{
    // Start is called before the first frame update

    private Train[] trains;
    public Train clientTrain { get; private set; }
    public bool FinishedSetup { get; private set; }

    private Section[] sections;
    private Turnout[] turnouts;
    private Waypoint firstPoint;
    
    public Transform spawn;
    public GameObject prefab;
    public SpedOMeter spedOMeter;
    public Client myClient;
    void Awake()
    {
        Debug.Log("Starting Track Awake");
        //spawn = s.position;
        //spawnTrain();
        Waypoint[] wps = FindObjectsOfType<Waypoint>();
        foreach (Waypoint w in wps) 
        { 
            if (w.id == 1)
            { 
                firstPoint = w; 
                break; 
            } 
        }

        sections = FindObjectsOfType<Section>();
        turnouts = FindObjectsOfType<Turnout>();
        //sorting
        Section[] _sects = new Section[sections.Length];
        Turnout[] _turns = new Turnout[turnouts.Length];

        foreach(Section s in sections)
        {
            _sects[s.order] = s;
        }
        sections = _sects;

        foreach (Turnout t in turnouts)
        {
            _turns[t.order] = t;
        }
        turnouts = _turns;

        trains = new Train[INetwork_Utils.MAX_TRAINS];//empty array to start

        //Debug.Log("number of sections found: " + sections.Length);
        //foreach (Section s in sections)
        //{
        //    Debug.Log("This sections id is: "+s.getID()+"\n"+"This sections path:"+s.getPath());
        //}
        Debug.Log("Finished Track Awake");
    }

    void Start()
    {
        Debug.Log("Starting Track Start()");
        DoClientSetup();
        Debug.Log("Finished Track Start() should have touched server");
    }

    // Update is called once per frame
    void Update()
    {
        if (!FinishedSetup)
            return;
        //Debug.Log("Entered Track Update()");
        foreach (Train t in trains)
        {
            if(!(t is null))
            {
                int sect = t.GetSectionIndex();
                if (t.isReady())//check if we need to get next
                {//getting next
                 //Debug.Log("Reached index: " + sections[i].myP.getCur() + " in section: " + i);
                    t.resetReady();
                    Waypoint next = sections[sect].Next();
                    if (next is null)//at end of path
                    {
                        changeSections(t, sections[sect].getNextSection(), sect);//train, where we are going, where we are/were
                    }
                    else//Do not need to leave yet
                        t.Move(next);
                }
                else
                    t.Move(sections[sect].getCur());//keep going at cur
            }
        }

    }

    private void changeSections(Train t, int next, int last)
    {
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

        if (!nextSection.isLocked())//this is the start of a critical region when working with race conditions this will need to be protected
        {
            #region Next Section Reverse or Not
            int sect_orientation;
            if (nextSection.getOrientationActual() == lastSection.myEndpoints[ep].Direction)
            { 
                nextSection.Reverse(false);
                sect_orientation = INetwork_Utils.CLOSED_NORM;
            }
            else
            { 
                nextSection.Reverse(true);
                sect_orientation = INetwork_Utils.CLOSED_REVERSE;
            }
                #endregion

            #region Restart Train
            if (t.isStopped())//direction you are entering from
            {
                    //Debug.Log("Train " + t.getID() + "is starting again");
                t.restart((int)spedOMeter.MySlider.value);
                spedOMeter.updateText();
            }
            #endregion

            #region Locking inaccessable turnouts
            if (lastSection.isReversed())
                turnoutAdjust(last, 1);//it entered at 1 and exited at 0 so 1 might close
            else
                turnoutAdjust(last, 0);
            #endregion

            lastSection.Exit();
            nextSection.Enter();

            t.SetSectionIndex(next);
            myClient.MySend(INetwork_Utils.ENTER + INetwork_Utils.DELIM + next + INetwork_Utils.DELIM + last + INetwork_Utils.DELIM + sect_orientation);
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
    private int SpawnTrainOnTrack(int trainID, int targetID, int sectionId, Vector3 position, string name)
    {
        Debug.Log("Spawning the other users trains");
        Waypoint target = sections[sectionId].GetWaypointByID(targetID);
        GameObject obj = new GameObject("");
        obj.transform.position = position;
        GameObject newObject = Instantiate(prefab, obj.transform);
        Train newTrain = newObject.GetComponent<Train>();
        bool added = false;
        int i;
        for (i = 0; i < trains.Length; i++)
        {
            if (trains[i] is null)
            {
                added = true;
                trains[i] = newTrain;
                trains[i].ID = trainID;
                trains[i].SetSectionIndex(sectionId);
                sections[sectionId].StartingWithinSectionAt(targetID);
                trains[i].Username.text = name;
                break;
            }
        }
        Debug.Log("finished with old train");
        if (added)
            return trains[i].ID;
        else
            return -1;
    }

    private void SpawnMyTrain()
    {

        //while (sections[0].isLocked())
        //{
        //write to UI waiting to spawn 
        //Debug.Log("couldnt spawn, waiting 5 sec");
        //  System.Threading.Thread.Sleep(5000);
        //}
        //if wrote to UI remove it and then complete spawn

        Debug.Log("Spawning this client's train");
        GameObject newObject = Instantiate(prefab, spawn);
        Train newTrain = newObject.GetComponent<Train>();
        if (clientTrain is null)
        { 
            clientTrain = newTrain;
            Debug.Log(PlayerPrefs.GetString("username"));
            clientTrain.Username.text = PlayerPrefs.GetString("username");
        }
        bool added = false;
        int i;
        for (i = 0; i < trains.Length; i++)
        {
            if (trains[i] is null)
            {
                added = true;
                trains[i] = newTrain;
                trains[i].SetTarget(firstPoint);
                break;
            }
        }
        sections[0].Enter();
        if (added)
        { myClient.MySend(INetwork_Utils.NEW_TRAIN + INetwork_Utils.DELIM + clientTrain.ID + INetwork_Utils.DELIM + clientTrain.Speed + INetwork_Utils.DELIM + clientTrain.Username.text); }
        else
        { Debug.Log("Couldnt add the train...track must be full"); }
    }
    private void SpawnOtherTrainSpawn(int id, int speed, string username)
    {

        //while (sections[0].isLocked())
        //{
        //write to UI waiting to spawn 
        //Debug.Log("couldnt spawn, waiting 5 sec");
        //  System.Threading.Thread.Sleep(5000);
        //}
        //if wrote to UI remove it and then complete spawn

        Debug.Log("New user joined");
        GameObject newObject = Instantiate(prefab, spawn);
        Train newTrain = newObject.GetComponent<Train>();
        int i;
        for (i = 0; i < trains.Length; i++)
        {
            if (trains[i] is null)
            {
                trains[i] = newTrain;
                trains[i].ID = id;
                trains[i].SpeedChange(speed);
                trains[i].Username.text = username;
                trains[i].SetTarget(firstPoint);
                break;
            }
        }
        sections[0].Enter();
    }
    public void ClientSpeedChanged() 
    {
        int s = (int)(spedOMeter.MySlider.value);
        clientTrain.SpeedChange(s);
        spedOMeter.updateText();
        myClient.MySend(INetwork_Utils.SPEED + INetwork_Utils.DELIM + clientTrain.ID + INetwork_Utils.DELIM + s);
    }

    public void HandleData(byte[] data)
    {
        string msg = ASCIIEncoding.ASCII.GetString(data);
        Debug.Log("made it to the handle data function");
        int i, val, act;
        string[] arr = msg.Split(INetwork_Utils.DELIM);
        if (!int.TryParse(arr[0], out act))
            Debug.Log("Message corrupted didnt find the action\n");

        switch (act)
        {
            #region Turnouts
            case INetwork_Utils.TURN://TURN;index;direction
                Debug.Log("In Turnout changed");
                if (arr.Length == 3 && int.TryParse(arr[1], out i) && int.TryParse(arr[2], out val))
                    AdjustTurnout(i, val);
                else
                {
                    Debug.Log("Message corrupted couldnt change turnout");
                    //tell the server
                }
                break;
            #endregion
            #region Speeds
            case INetwork_Utils.SPEED://SPEED;index;speed
                Debug.Log("In Speed change");
                if (arr.Length == 3 && int.TryParse(arr[1], out i) && int.TryParse(arr[2], out val))
                    ChangeSpeed(i, val);
                else
                {
                    Debug.Log("Message corrupted couldnt update speed");
                    //tell the server
                }
                break;
            #endregion
            #region Section Change CODE NOT WRITTEN
            case INetwork_Utils.ENTER://ENTER;index in;index out
                break;
            #endregion
            #region Location Requested
            case INetwork_Utils.LOC:
                SendLocation();
                break;
            #endregion
            #region New Train
            case INetwork_Utils.NEW_TRAIN://NEW_TRAIN;id;speed;name
                Debug.Log("In New Train");
                i = int.Parse(arr[1]);
                val = int.Parse(arr[2]);
                string name;
                if (arr.Length == 4)
                    name = arr[3];
                else
                    name = "Didnt get the name";
                SpawnOtherTrainSpawn(i,val,name);

                break;
            #endregion
            #region Disconnection
            case INetwork_Utils.DISCONNECT:
                RemoveTrain(int.Parse(arr[1]),int.Parse(arr[2]));
                break;
            #endregion
        }
    }
    private void DoClientSetup()
    {
        myClient.MySend(INetwork_Utils.NONE);//send an empty message just to establish a connection
        string turns = myClient.MyRecieve();
        if ((turns.Split(INetwork_Utils.DELIM + ""))[0].Equals( INetwork_Utils.DISCONNECT + ""))
            return;//couldnt connect
        string sects = myClient.MyRecieve();
        string trs = myClient.MyRecieve();

        SetupSections(sects);
        SetupTurns(turns);
        SetupTrains(trs);
        FinishedSetup = true;
        Debug.Log("Finished Client Setup");
        //start the client train iff section 0 is open 
    }

    private void SetupSections(string data)
    {
        string[] splitData = data.Split(INetwork_Utils.DELIM);
        string msg="";
        foreach (string s in splitData)
            msg += " " + s;
        //Debug.Log("section data after split " + msg);
        for (int i = 0; i < splitData.Length-1; i++)
        {
            int sect_data = int.Parse(splitData[i]);
            if (sect_data != INetwork_Utils.OPEN)
            {
                if (sect_data == INetwork_Utils.CLOSED_REVERSE)
                    sections[i].Reverse(true);
                sections[i].Lock(); 
            }
            else
                sections[i].Unlock();
        }
    }
    private void SetupTurns(string data)
    {
        string[] splitData = data.Split(INetwork_Utils.DELIM);
        for (int i = 0; i < splitData.Length; i++)
        {
            int val;
            if(int.TryParse(splitData[i], out val))
                AdjustTurnout(i, val);
        }
    }

    private void SetupTrains(string data)
    {//train id;target id;section id;Vector3 dir (x,y,z);Username
        Debug.Log(data);
        if (data.Equals(INetwork_Utils.NONE))
        { 
            SpawnMyTrain();//no other trains on track
            return;
        }

        string[] trainData = data.Split(INetwork_Utils.DELIM);
        //int trainID = int.Parse(trainData[0]);
        //int waypointID = int.Parse(trainData[1]);
        //int sectionID = int.Parse(trainData[2]);
        //string[] splitString = trainData[3].Trim(new char[] { '(', ')' }).Split(',');
        //float x, y, z;
        //Vector3 dir;
        //if (float.TryParse(splitString[0], out x) && float.TryParse(splitString[1], out y) && float.TryParse(splitString[2], out z))
        //{
        //    Debug.Log("Got the x y z correctly");
        //    dir = new Vector3(x, y, z);
        //}
        //else
        //{
        //    Debug.LogError("Failed to get the x y z not spawning the train");
        //    return;
        //}
        //string name;
        //if (trainData.Length == 5)
        //    name = trainData[4]; 
        //else
        //    name = "Default Train Name";

        for (int i = 0; i < trainData.Length - 5; i += 5)//train id; waypoint id; section id; Vector3 position; username 
        {
            int trainID = int.Parse(trainData[i]);
            int waypointID = int.Parse(trainData[i + 1]);
            int sectionID = int.Parse(trainData[i + 2]);
            string[] splitString = trainData[i + 3].Trim(new char[] { '(', ')' }).Split(',');
            Vector3 position = new Vector3(float.Parse(splitString[0]), float.Parse(splitString[1]), float.Parse(splitString[2]));
            string name;
            try { name = trainData[i + 4]; } catch (IndexOutOfRangeException) { name = "name did not send"; }
            SpawnTrainOnTrack(trainID, waypointID, sectionID, position, name);
        }
        //SpawnTrainOnTrack(trainID, waypointID, sectionID, dir, name);
        SpawnMyTrain();//spawn other train then ours


        
    }
    private void AdjustTurnout(int id, int dir)
    {
        Turnout changed = turnouts[id];
        changed.RotateTurnout(dir, changed.Direction);
        changed.Direction = dir;
    }

    private void ChangeSpeed(int id, int s)
    {
        foreach (Train t in trains)
        {
            if (!(t is null)&&t.ID == id)
            { t.SpeedChange(s); Debug.Log("found the train changing speed"); }
        }
    }

    private void SendLocation()
    {
        myClient.MySend(clientTrain.getLocation());
    }

    private void RemoveTrain(int id, int section_id) 
    {
        for (int i = 0; i < trains.Length; i++)
        {
            if (!(trains[i] is null) && trains[i].ID == id)
            {
                Debug.Log("Found the train to remove");
                Train disconnected = trains[i];
                Destroy(disconnected.transform.gameObject);
                Debug.Log("Child count of the transform.parent " + disconnected.transform.parent.childCount);
                trains[i] = null;
                if (trains[i] is null)
                    Debug.Log("Set train " + i + "to null");
                else
                    Debug.Log("null didnt work");
                //make sure to clear the section

                sections[section_id].Exit();
            }
        }
    }

    public void Turned(int index, int dir)
    {
        myClient.MySend(INetwork_Utils.TURN + INetwork_Utils.DELIM + index + INetwork_Utils.DELIM + dir);
    }
}
