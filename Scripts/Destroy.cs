using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destroy : MonoBehaviour
{

    UIManager ui;
    private void Awake()
    {
        ui = FindAnyObjectByType<UIManager>();
    }

    private void Start()
    {
        ui.coin += 1;
        ui.score += 100;
    }

    void DestroyGameObject()
	{
		// 이 게임오브젝트 삭제
		Destroy(transform.parent.gameObject);
	}
}
