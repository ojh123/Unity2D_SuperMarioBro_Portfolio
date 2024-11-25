using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Coin = 0,
    SizeUp = 1,
    LifeUp,
    Star,
    Flower,
}

public class QuestionBlock : MonoBehaviour
{
    public ItemType itemType = ItemType.Coin;
    public GameObject[] itemPrefabs;
    Animator anim;
    
    PlayerCtrl pctrl;
    void Awake()
    {
        anim = GetComponent<Animator>();
        pctrl = FindObjectOfType<PlayerCtrl>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemCollider") && this.GetComponent<QuestionBlock>().enabled == true)
        {
            anim.SetBool("Touch", true);
            this.GetComponent<QuestionBlock>().enabled = false; // 충돌 후 스크립트를 비활성화
            switch (itemType)
            {
                case ItemType.Coin:
                    {
                        Touch(ItemType.Coin);
                        SoundManager.instance.PlaySfx("coin");
                        break;
                    }
                case ItemType.SizeUp:
                    {
                        if (pctrl.sizeUp == false)
                        {
                            StartCoroutine(TouchDelay(ItemType.SizeUp, 0.3f));
                        }
                        else if( pctrl.sizeUp == true)
                        {
                            StartCoroutine(TouchDelay(ItemType.Flower, 0.3f));
                        }

                        break;
                    }
                case ItemType.LifeUp:
                    {
                        StartCoroutine(TouchDelay(ItemType.LifeUp, 0.3f));
                        break;
                    }
                case ItemType.Star:
                    {
                        StartCoroutine(TouchDelay(ItemType.Star, 0.3f));
                        break;
                    }

            }

        }
    }

    void Touch(ItemType item)
    {
        Instantiate(itemPrefabs[(int)item], transform.position, transform.rotation);
    }

    IEnumerator TouchDelay(ItemType item, float delay)
    {
        yield return new WaitForSeconds(delay);
        Touch(item);
    }
}
