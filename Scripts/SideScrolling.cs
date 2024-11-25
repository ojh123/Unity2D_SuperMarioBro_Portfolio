using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    private Transform player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate() // ������Ʈ �� ī�޶� ���󰡰� 
    {
        Vector3 cameraPosition = transform.position;
        // ī�޶��� x ��ǥ�� �÷��̾��� x ��ǥ���� ������ �� ���� �÷��̾��� x ��ǥ�� ������Ʈ
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
        transform.position = cameraPosition;
    }
}