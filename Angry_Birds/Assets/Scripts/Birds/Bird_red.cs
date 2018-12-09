using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird_red : MonoBehaviour {

    bool canUseAbility = true;   //是否能使用能力
    Rigidbody2D rb;

    AudioSource audioSource;
    public AudioClip[] crySound;
    public AudioClip[] collisionSound;
    
	void Start () {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update () {
        if (GameManager.cursorInLevel && gameObject.name.EndsWith(GameManager.birdNum.ToString()) && Input.GetMouseButtonDown(0) && Birds.state==Birds.BirdState.Shot && rb.bodyType==RigidbodyType2D.Dynamic && canUseAbility && GameManager.gameState != GameManager.GameState.End)
        {
            canUseAbility = false;
            int cryRand = Random.Range(0, 3);
            if (cryRand == 3) cryRand = 2;
            audioSource.PlayOneShot(crySound[cryRand]);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name.EndsWith(GameManager.birdNum.ToString()) && Birds.state == Birds.BirdState.Shot)
        {
            canUseAbility = false;
            if (GetComponent<Rigidbody2D>().velocity.magnitude >= 4f)
            {
                int birdCollsionRand = Random.Range(0, 4);
                if (birdCollsionRand == 4) birdCollsionRand = 0;
                audioSource.PlayOneShot(collisionSound[birdCollsionRand]);
            }
        }
    }
}
