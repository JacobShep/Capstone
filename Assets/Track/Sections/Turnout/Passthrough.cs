using UnityEngine;
public class Passthrough : Endpoint
{
    private void Start() 
    {
        swapDirections();
    }
    public void swapDirections()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i != direction && destinations[i] != -1)
            {
                Debug.Log("Swapping " + direction + " with " + i);
                direction = i;
                return;
            }
        }
    }
}
