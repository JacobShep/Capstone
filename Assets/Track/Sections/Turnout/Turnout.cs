using UnityEngine;
public class Turnout : Endpoint
{
    private void Awake()
    {
        for (int i = 0; i < destinations.Length; i++)//set direction to the first valid 
        {
            if (destinations[i] != -1)
            { direction = i; break; }
        }
        rotate(direction, 2);//use two because the default arrow sprite faces down
    }
    private void OnMouseUp()//when this collider is clicked change the direction
    {
        if (active)
        {
            int wasFacing = direction;
            Debug.Log("clicked");
            do//move it one over and repeat if not valid direction
            {
                if (direction < destinations.Length - 1)
                    direction++;
                else
                    direction = 0;
            } while (destinations[direction] == -1);

            string str = "";
            switch (direction)
            {
                case IDirections.UP: str = "up"; break;
                case IDirections.RIGHT: str = "right"; break;
                case IDirections.DOWN: str = "down"; break;
                case IDirections.LEFT: str = "left"; break;
            }
            Debug.Log("Now pointing " + str);

            //after it decides the correct direction it needs to update the sprite to look correct
            rotate(direction, wasFacing);
        }
    }

    private void rotate(int dir, int wf)
    {
        if (dir != wf)//if it changed
        {
            Quaternion rotation = transform.localRotation;
            int dif = wf - dir;
            int degrees = dif * 90;
            Vector3 v = new Vector3(0f, 0f, (float)degrees);
            transform.Rotate(v, Space.World);
            Debug.Log("Rotated by " + degrees + " degrees");
        }
    }
}
