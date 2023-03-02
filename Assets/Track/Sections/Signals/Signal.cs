using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer r;
    public Sprite c, o;
    private void Awake()
    {
        r.sprite = o;
    }

    public void open() { r.sprite = o; }
    public void close() { r.sprite = c; }
}
