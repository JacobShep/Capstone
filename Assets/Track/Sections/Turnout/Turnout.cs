using UnityEngine;
public class Turnout : Endpoint
{
    public int order;
    private Track theTrack;
    private void Awake()
    {
        theTrack = FindObjectOfType<Track>();
        Direction = 2;
    }
    private void OnMouseUp()//when this collider is clicked change the direction
    {
        if (active)
        {
            int wasFacing = Direction;
            do//move it one over and repeat if not valid direction
            {
                if (Direction < destinations.Length - 1)
                    Direction++;
                else
                    Direction = 0;
            } while (destinations[Direction] == -1);

            //after it decides the correct direction it needs to update the sprite to look correct
            RotateTurnout(Direction, wasFacing);
            theTrack.Turned(order, Direction);
        }
    }

    

    public void RotateTurnout(int dir, int wasFacing)
    {
        Debug.Log("Turning " + dir + " to " + wasFacing);
        if (dir != wasFacing)//if it changed
        {
            int dif = wasFacing - dir;
            int degrees = dif * 90;
            Vector3 v = new Vector3(0f, 0f, (float)degrees);
            transform.Rotate(v, Space.World);
        }
    }
}
