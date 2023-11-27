using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [Header ("References")]
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private Canvas chargeBar;
    [SerializeField] private RectTransform chargeFill; 
    [SerializeField] private RectTransform chargeBorder;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private Image timerImage;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform zSpawner; 
    [SerializeField] private GameObject zPrefab;

    [Header ("Attributes")]
    [SerializeField] private Vector2 jumpAngle;
    [SerializeField] private float jumpBuffer;
    [SerializeField] private Vector2 forceFactor;
    [SerializeField] private float chargeFactor;
    [SerializeField] public float maxStrength;
    [SerializeField] private float jumpStrengthInit;
    [SerializeField] private float scaleTime;
    [SerializeField] public Vector3[] scales;
    [SerializeField] private float moveThreshold;
    [SerializeField] private float zSpawnTime;

    [Header ("Tags")]
    [SerializeField] private string groundTag;
    [SerializeField] private string deadZoneTag;
    [SerializeField] private string crownTag;


    [Header ("Audio")]
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource chargeSound;
    [SerializeField] private AudioSource maxChargeSound;
    [SerializeField] private AudioSource scaleSoundToSmall;
    [SerializeField] private AudioSource scaleSoundToLarge;
    [SerializeField] private float hitSoundTime;


    [Header ("Checks")]
    [SerializeField] private bool log;

    // jump variables
    public Vector2 facing = Vector2.one;
    private Vector2 jumpForce;
    private float jumpCharge;
    public float jumpStrength;
    private bool jump;
    private float jumpTimer;
    private bool grounded;
    private bool isMoving;
    public bool landed;
    public bool charging;
    private bool pressedSpace;
    private bool isTouching;

    // scale variables
    private Vector3 initScale;
    private bool rescale;
    private float scaleTimer;
    public int scaleIndex;
    private float timerFill;

    // audio variables
    
    private float hitSoundTimer;
    private bool hitSoundPlayed;
    private bool chargeSoundPlayed;
    private bool maxChargeSoundPlayed;

    // game state variables
    public bool dead;
    public bool won;
    private float zSpawnTimer;

    void Start()
    {
        jumpAngle = jumpAngle.normalized;
        initScale = scales[0];
        transform.localScale = initScale;
        won = false;
        dead = false;
        zSpawnTimer = zSpawnTime - 0.1f;
    }

    void Update()
    {
        if (gameManager.awake)
        {
            JumpControl();
            ScaleTimer();
            HasLanded();
            Facing();
            if (rescale) 
            {
                ScalePlayer();
            }
            if (hitSoundPlayed)
            {
                HitSoundTimer();
            }
            PlayChargeSounds();
        }
        else
        {
            ScaleTimer();
            if (rescale) 
            {
                ScalePlayer();
            }
            ZSpawner();
        }
        
    }
    
    void FixedUpdate()
    {
        if (gameManager.awake)
        {
            ChargeBar();
        if (jump)
        {
            Jump();
        }
        }
        
    }

    private void Jump()
    {
        jumpTimer += Time.fixedDeltaTime;
        float incJumpBuffer = jumpBuffer * jumpStrength;

        if (jumpTimer > incJumpBuffer)
        {
            jump = false;
            jumpStrength = 0f;
            jumpTimer = 0f;
            jumpCharge = 0f;
            incJumpBuffer = jumpBuffer;
        }
        else if (landed)
        {
            jumpForce = Vector2.Scale(facing, jumpAngle) * forceFactor[scaleIndex] * jumpStrength;
            rb2d.AddForce(jumpForce, ForceMode2D.Impulse);
            jumpSound.Play();
            jump = false;
            jumpStrength = 0f;
            jumpTimer = 0f;
            jumpCharge = 0f;
            incJumpBuffer = jumpBuffer;
        }
        
        
        
    }

    private void JumpControl()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            charging = true;
        }

        if (charging)
        {
            jumpCharge += Time.deltaTime;
            if (log)
            {
                Debug.Log("what is the initial Strength: " + jumpStrengthInit + ", and what is the jump charge? " + jumpCharge + ", and the chargeFactor? " + chargeFactor);
            }
            jumpStrength = Mathf.Max((jumpStrengthInit + jumpCharge * chargeFactor), 1f);
            jumpStrength = Mathf.Min(jumpStrength, maxStrength);
            if (log)
            {
                Debug.Log("and what is it actually? " + jumpStrength);
            }
        }
        

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jump = true;
            charging = false;
        }
    }

    private void ChargeBar()
    {
        float maxWidth = chargeBorder.rect.width;
        float height = chargeBorder.rect.height;

        chargeFill.sizeDelta = new Vector2(Map(jumpStrength, 1f, maxStrength, 0f, maxWidth), height);
    }

    private void Facing() 
    {
        if (!isTouching && Mathf.Abs(rb2d.velocity.x) > 0.5f)
        {
            //facing = new Vector2(Mathf.Sign(Mathf.Round(rb2d.velocity.x * 10f) / 10f), 1f);
            facing = new Vector2(Mathf.Sign(rb2d.velocity.x), 1f);
        }
    }

    private void ScaleTimer()
    {
        scaleTimer += Time.deltaTime;
        timerFill = Map(scaleTimer, 0f, scaleTime, 1f, 0f);
        timerImage.fillAmount = timerFill;
        if (scaleTimer > scaleTime)
        {
            rescale = true;
            scaleTimer = 0f;
        }
    }

    private void ScalePlayer()
    {
        scaleIndex += 1;
        scaleIndex = scaleIndex % 2;
        if (scaleIndex == 0)
        {
            scaleSoundToSmall.Play();
        }
        else
        {
            scaleSoundToLarge.Play();
        }
        transform.localScale = scales[scaleIndex];
        rescale = false;
        

    }

    private void HasLanded()
    {
        if (rb2d.velocity.y < 0.05f && Mathf.Abs(rb2d.velocity.x) < 1.5f)
        {
            landed = true;
        }
        else 
        {
            landed = false;
        }
    }

    private float Map(float yValue, float yMin, float yMax, float xMin, float xMax)
    {
        float xDelta = xMax - xMin;
        float yDelta = yMax - yMin;
        
        // Ensure that yDelta is not zero to avoid division by zero
        if (yDelta != 0)
        {
            return (yValue - yMin) * (xDelta / yDelta) + xMin;
        }
        else
        {
            return 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hitSoundPlayed == false)
        {
            hitSound.Play();
            hitSoundPlayed = true;
        }
        isTouching = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isTouching = false;
    }

    private void HitSoundTimer()
    {
        hitSoundTimer += Time.deltaTime;
        if (hitSoundTimer > hitSoundTime)
        {
            hitSoundPlayed = false;
            hitSoundTimer = 0f;
        }
    }

    private void PlayChargeSounds()
    {
        if (!chargeSoundPlayed && charging)
        {
            chargeSound.Play();
            chargeSoundPlayed = true;
        } else if (!charging)
        {
            chargeSound.Stop();
        }
        if (!maxChargeSoundPlayed && jumpStrength == maxStrength && !jump)
        {
            maxChargeSound.Play();
            maxChargeSoundPlayed = true;
        }
        if (jump)
        {
            chargeSoundPlayed = false;
            maxChargeSoundPlayed = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(deadZoneTag))
        {
            dead = true;
        }
        if (other.CompareTag(crownTag))
        {
            won = true;
        }
    }   

    private void ZSpawner()
    {
        zSpawnTimer += Time.deltaTime;
        if (zSpawnTimer > zSpawnTime)
        {
            Instantiate(zPrefab, zSpawner.position, Quaternion.identity);
            zSpawnTimer = 0f;
        }
        
    }
}

