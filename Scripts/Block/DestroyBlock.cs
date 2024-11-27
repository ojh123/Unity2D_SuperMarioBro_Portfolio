using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    
    void Start()
    {
        Destroy(this.gameObject, 1.0f);
        SoundManager.instance.PlaySfx("breakblock");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
