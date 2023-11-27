using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private Transform crownTransform;
    [SerializeField] private Vector3 endCrownScale;
    [SerializeField] private Canvas scaleTimerCanvas;
    [SerializeField] private SpriteRenderer crownSprite;

    [Header ("Attributes")]
    [SerializeField] private float zoomWait;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float lerpDuration = 2f;
    [SerializeField] private float blackLerpDuration = 10f;
    [SerializeField] private float blackWaitTime;
    [SerializeField] private float lerpTime = 0f;
    [SerializeField] private float[] snorePitches;
    [SerializeField] private float[] snoreVolumes;

    [Header ("Audio")]
    [SerializeField] private AudioSource snoringSound;
    [SerializeField] private AudioSource awakeSound;
    
    private float zoomTimer;
    public bool awake;
    private Vector3 initCrownScale;
    private float blackWaitTimer;

    void Start()
    {
        initCrownScale = crownTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !awake)
        {
            awake = true;
            snoringSound.Stop();
            awakeSound.Play();
        }

        if (playerController.dead)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (playerController.won)
        {
            rb2d.velocity = Vector3.zero;
            playerController.enabled = false;
            rb2d.gravityScale = 0;
            scaleTimerCanvas.enabled = false;
            CrownZoom();
            BlackScreen();
        }
        SnorePitch();
        
    }

    private void CrownZoom()
    {
        if (lerpTime < lerpDuration)
        {
            lerpTime += Time.deltaTime;

            // Interpolate between startScale and endScale based on lerpTime
            crownTransform.localScale = Vector3.Lerp(initCrownScale, endCrownScale, lerpTime / lerpDuration);
        }
    }

    private void BlackScreen()
    {
        
        blackWaitTimer += Time.deltaTime;
        if (blackWaitTimer > blackWaitTime)
        {
            float lerpFactor = Mathf.Clamp01((blackWaitTimer - blackWaitTime) / blackLerpDuration);
            // Lerp the color from white to black
            Color lerpedColor = Color.Lerp(Color.white, Color.black, lerpFactor);

            // Assign the lerped color to the sprite renderer's color
            crownSprite.color = lerpedColor;
        }
    }

    private void SnorePitch()
    {
        snoringSound.pitch = snorePitches[playerController.scaleIndex];
        snoringSound.volume = snoreVolumes[playerController.scaleIndex];
    }
}
