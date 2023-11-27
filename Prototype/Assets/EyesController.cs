using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesController : MonoBehaviour
{

     [Header ("References")]
    [SerializeField] Transform playerPos;
    [SerializeField] Transform playerScale;
    [SerializeField] private Sprite idleEyes;
    [SerializeField] private Sprite chargeEyes;
    [SerializeField] private Sprite maxChargeEyes;
    [SerializeField] private Sprite flyEyes;
    [SerializeField] private Sprite sleepEyes;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameManager gameManager;

    [Header ("Attributes")]
    [SerializeField] private Vector2 offset;
    [SerializeField] private float yEyes;
    [SerializeField] private float idleOffset;

    private Vector3 eyesScale;
    private Vector3 nextPos;
    private Vector3 nextScale;
    private bool maxCharge;
    private bool idle;


    // Start is called before the first frame update
    void Start()
    {
        eyesScale = transform.localScale;
        spriteRenderer.sprite = sleepEyes;
    }

    // Update is called once per frame
    void Update()
    {
            UpdatePosition(offset);
            UpdateScale();
            ChangeEyesSprite();      
    }


    private void UpdatePosition(Vector3 offset)
    {
        transform.position = playerPos.position + offset;
        Debug.Log(playerController.scales[playerController.scaleIndex][0]);
        if (idle)
        {
            transform.position = playerPos.position + offset + new Vector3(playerController.facing.x * playerController.scales[playerController.scaleIndex][0] * idleOffset, 0f, 0f);
        }
        
    }

    private void UpdateScale()
    {
        transform.localScale = new Vector3(eyesScale.x * playerScale.localScale.x, eyesScale.y * playerScale.localScale.y, eyesScale.z * playerScale.localScale.z);
    }

    private void ChangeEyesSprite()
    {
        if (playerController.charging)
        {
            spriteRenderer.sprite = chargeEyes;
            if (playerController.jumpStrength == playerController.maxStrength)
            {
                spriteRenderer.sprite = maxChargeEyes;
            }
            idle = false;
        }
        else if (playerController.landed)
        {
            spriteRenderer.sprite = idleEyes;
            idle = true;
        }
        else if (!gameManager.awake)
        {
            spriteRenderer.sprite = sleepEyes;
            idle = false;
        }
        else
        {
            spriteRenderer.sprite = flyEyes;
            idle = false;
        }
    }
}
