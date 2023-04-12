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
            if (i != Direction && destinations[i] != -1)
            {
                Debug.Log("Swapping " + Direction + " with " + i);
                Direction = i;
                return;
            }
        }
    }
}
