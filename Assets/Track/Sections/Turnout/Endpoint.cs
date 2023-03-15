using UnityEngine;
abstract public class Endpoint : MonoBehaviour
{
    public int[] destinations = new int[4];
    protected bool active = true;
    protected int direction;
    public int getNext() { return destinations[direction]; }

    public void deactivate() { active = false; }
    public void activate() { active = true; }
    
    public bool isActive() { return active; }
    
    public int[] getSections() { return destinations; }

    public int getDirection() { return direction; }
}
