using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowsPlayer : MonoBehaviour
{
    public GameObject Background;
    Material test;
    public Transform player;
    public float speed = 5;
    public float mapSpeed = 0.5f;
    public float cameraHeight = 3;
    void Start()
    {
        test = Background.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (player)
        {
            Vector2 move = Vector2.Lerp(transform.position, new Vector2(player.position.x, player.position.y+ cameraHeight), Time.deltaTime * speed);
            test.mainTextureOffset = move* -mapSpeed;
            transform.position = new Vector3(move.x, move.y, -10);
        }
    }
}
