using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using Battlehub.Dispatcher;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update
    private UdpClient socket;
    private bool listening = false;
    private Dispatcher dispatcher;
    public Track track;
    private IPEndPoint serverEp;
    void Awake()
    {
        Debug.Log("Starting Client Awake");
        dispatcher = FindObjectOfType<Dispatcher>();
        socket = new UdpClient();
        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, INetwork_Utils.CLIENT_PORT_NUM);
        socket.Client.ReceiveTimeout = INetwork_Utils.TIMEOUT;
        IPAddress serverAddress = Dns.GetHostAddresses(INetwork_Utils.SERVER_DOMAIN_NAME)[0];
        //IPAddress serverAddress = IPAddress.Parse(INetwork_Utils.SERVER_ADDRESS);
        serverEp = new IPEndPoint(serverAddress, INetwork_Utils.SERVER_PORT_NUM);
        if (socket is null)
            Debug.Log("Socket null");
        if (serverEp is null)
            Debug.Log("server end point is null");
        Debug.Log("Finished Client Awake");
    }

    void OnApplicationQuit()
    {
        MySend(INetwork_Utils.DISCONNECT+INetwork_Utils.DELIM+track.clientTrain.ID+INetwork_Utils.DELIM+track.clientTrain.GetSectionIndex());
        PlayerPrefs.DeleteAll();
    }

    public void MySend(string msg)
    {
        Debug.Log("Sending message: " + msg);
        byte[] data = Encoding.ASCII.GetBytes(msg);
        socket.Send(data, data.Length, serverEp);
        
    }

    public string MyRecieve() 
    {
        byte[] data = socket.Receive(ref serverEp);
        string msg = Encoding.ASCII.GetString(data);
        Debug.Log("Recieved Message: " + msg);
        return msg;
    }

    // Update is called once per frame
    void Update()
    {
        if (track.FinishedSetup && !listening)
        {
            listening = true;
            Debug.Log("Restarting the lambda");
            startAsyncListen();
        }
    }

    private async void startAsyncListen()
    {
        UdpReceiveResult result = await socket.ReceiveAsync();
        listening = false;
        Debug.Log("listener hit!");
        byte[] data = result.Buffer;
        dispatcher.BeginInvoke(()=>{ 
            Debug.Log("in the lambda!\n");
            Debug.Log("Lambda Recieved: " + Encoding.ASCII.GetString(data));
            track.HandleData(data); });
    }
}
