using UnityEngine;
abstract public class Endpoint : MonoBehaviour
{
    public int[] destinations = new int[4];
    protected bool active = true;
    public int Direction{get; set;}

    public int getNext() {
        return destinations[Direction];
    }

    public void deactivate() { active = false; }
    public void activate() { active = true; }
    
    public bool isActive() { return active; }
    
    public int[] getSections() { return destinations; }
}
