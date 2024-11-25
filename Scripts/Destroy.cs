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
		// �� ���ӿ�����Ʈ ����
		Destroy(transform.parent.gameObject);
	}
}
