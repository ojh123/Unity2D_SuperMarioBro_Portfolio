using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : ItemUp
{
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = collision.gameObject.GetComponent<PlayerCtrl>();
            playerCtrl.FlowerMario();
        }
    }

}
