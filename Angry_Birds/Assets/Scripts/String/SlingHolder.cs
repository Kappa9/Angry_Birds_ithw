using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingHolder : MonoBehaviour {

    Vector3 startPos;
    SpriteRenderer sr;
    Rigidbody2D rb;
    AudioSource audioSource;

    void Start () {
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }
	
	void FixedUpdate () {
        if (Birds.state == Birds.BirdState.Grabbed && GameManager.gameState != GameManager.GameState.End)
        {
            rb.position = SlingString.birdPos;
            transform.rotation = Quaternion.Euler(180f, 0f, Vector2.SignedAngle(SlingString.delta, Vector2.up)+75);
            transform.localScale = new Vector3(1.5f, 1.5f, 0);
            sr.sortingOrder = 3;
        }
        else
        {
            rb.position = startPos;
            transform.rotation = Quaternion.Euler(180, 0, 75);
            transform.localScale = new Vector3(1, 1, 0);
            sr.sortingOrder = 0;
        }
        if (Birds.state == Birds.BirdState.Grabbed && GameManager.gameState != GameManager.GameState.End)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else audioSource.Stop();
    }
}
