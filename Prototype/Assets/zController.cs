using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zController : MonoBehaviour
{
    [Header ("Attributes")]
    [SerializeField] private float destroyTime;
    [SerializeField] private Vector3 endPos;

    private float lerpTime;
    private Vector3 initPos;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        Destroy(gameObject, destroyTime);
        GameObject gameManagerObj = GameObject.Find("Game Manager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveZ();
        if (gameManager.awake)
        {
            Destroy(gameObject);
        }
    }

    private void MoveZ()
    {
        lerpTime += Time.deltaTime;
        transform.position= Vector3.Lerp(initPos, initPos + endPos, lerpTime / destroyTime);
    }

}
