using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird_Yellow : MonoBehaviour {

    bool canUseAbility = true;   //是否能使用能力
    Rigidbody2D rb;

    public AnimationClip boostAnimation;
    AnimationClip currentClip;

    AudioSource audioSource;
    public AudioClip abilitySound;
    public AudioClip[] collisionSound;

    public GameObject smoke_Ability;
    GameObject smoke;
    
    void Start () {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.cursorInLevel && gameObject.name.EndsWith(GameManager.birdNum.ToString()) && Input.GetMouseButtonDown(0) && Birds.state == Birds.BirdState.Shot && rb.bodyType == RigidbodyType2D.Dynamic && canUseAbility && GameManager.gameState != GameManager.GameState.End)
        {
            canUseAbility = false;
            smoke = Instantiate(smoke_Ability, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(abilitySound);
            rb.velocity = rb.velocity.normalized * 20f;
        }
        if (!canUseAbility&&GetComponent<TrailRenderer>()==null)
        {
            Destroy(smoke);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name.EndsWith(GameManager.birdNum.ToString()) && Birds.state == Birds.BirdState.Shot)
        {
            canUseAbility = false;
            if (GetComponent<Rigidbody2D>().velocity.magnitude >= 4f)
            {
                int birdCollsionRand = Random.Range(0, 6);
                if (birdCollsionRand == 6) birdCollsionRand = 0;
                audioSource.PlayOneShot(collisionSound[birdCollsionRand]);
            }
        }
    }
}
