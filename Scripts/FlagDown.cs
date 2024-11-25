using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagDown : MonoBehaviour
{
    public Transform flag;
    bool move;
    
    // Update is called once per frame
    void Update()
    {
        if(move && flag.position.y >= -0.52)
        {
            flag.position += Vector3.down * Time.deltaTime * 0.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            move = true;
        }
    }
}
