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

    private void LateUpdate() // 업데이트 후 카메라 따라가게 
    {
        Vector3 cameraPosition = transform.position;
        // 카메라의 x 좌표가 플레이어의 x 좌표보다 작으면 그 값을 플레이어의 x 좌표로 업데이트
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
        transform.position = cameraPosition;
    }
}