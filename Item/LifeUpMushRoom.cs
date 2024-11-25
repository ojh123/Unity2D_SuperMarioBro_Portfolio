using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUpMushRoom : ItemUp
{
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (moveOn)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Star"))
        {
            SoundManager.instance.PlaySfx("life-up");
        }

        if (collision.gameObject.layer == 10)
        {
            speed *= -1;
        }
    }
   
}
