using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public interface INetwork_Utils
{
    public const string SERVER_DOMAIN_NAME = "compsci04.snc.edu";
    public const string SERVER_ADDRESS = "127.0.0.1";
    public const int SERVER_PORT_NUM = 3949;
    public const int CLIENT_PORT_NUM = 12001;
    public const int TIMEOUT = 2500;
    public const int MAX_TRAINS = 8;

    public const int SPEED = 0;
    public const int TURN = 1;
    public const int ENTER = 2;
    public const int GET = 3;
    public const int DISCONNECT = 4;
    public const int LOC = 5;
    public const int NEW_TRAIN = 6;
    public const string NONE = "-";
    public const string DELIM = ";";


    public const int OPEN = 0;
    public const int CLOSED_NORM = 1;
    public const int CLOSED_REVERSE = 2;
}
