using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bricks : MonoBehaviour
{
    Animator anim;
    public GameObject particle;
    PlayerCtrl pctrl;

    void Awake()
    {
        anim = GetComponent<Animator>();
        pctrl = FindObjectOfType<PlayerCtrl>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemCollider") && pctrl.sizeUp == false)
        {
            anim.SetTrigger("Touch");
            SoundManager.instance.PlaySfx("bump");
        }

        if (pctrl.sizeUp == true && collision.CompareTag("ItemCollider"))  // ������� ���¸� ��� �ı�
        {
            Instantiate(particle, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }

    }
}
