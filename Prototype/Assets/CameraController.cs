using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private Transform player;
    // Start is called before the first frame update

    private float endOffsetY;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float playerY = Mathf.Floor((player.position.y + 6f) / 12f);
        float playerX = Mathf.Floor((player.position.x + 8f) / 16f);
        transform.position = new Vector3(0, playerY * 12, -10);
        if (player.position.x > 8)
        {
            transform.position = new Vector3(playerX * 16, 106, -10);
        }
    }
}
