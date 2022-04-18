using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{

    Camera _camera;
    CapsuleCollider _controller;
    public float smoothness = 10f;

    void Start()
    {
        _camera = Camera.main;
        _controller = this.GetComponent<CapsuleCollider>();
    }

    
    void Update()
    {
        
    }

    void LateUpdate()  //플레이어가 카메라를 바라봄
    {
        Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
    }
}
