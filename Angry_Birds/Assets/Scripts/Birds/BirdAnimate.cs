using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimate : MonoBehaviour {

    private Animator animator;
    private AudioSource audioSource;
    public GameObject parent;

    public AudioClip[] crysound;
    float nextchange, changerate;

    void Start() {
        changerate = Random.Range(0, 5);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.SetTrigger("Idle");
        nextchange = Time.time + changerate;
    }
    
    void Update () {
        if(Time.time > nextchange)
        {
            if (parent.name.EndsWith(GameManager.birdNum.ToString()) && (Birds.state == Birds.BirdState.Idle || Birds.state == Birds.BirdState.OnString))
            {
                changerate = Random.Range(2, 4);
                RandAnimation();
            }
            else if (!parent.name.EndsWith(GameManager.birdNum.ToString()))
            {
                changerate = Random.Range(2, 4);
                RandAnimation();
            }
        }
    }

    void RandAnimation()
    {
        int rand = Random.Range(0, 100);
        if (rand >= 0 && rand < 10)
        {
            animator.SetTrigger("Blink");
            nextchange = Time.time + changerate;
        }
        else if (rand >= 10 && rand < 30)
        {
            animator.SetTrigger("Cry");
            nextchange = Time.time + changerate;
        }
        else if (rand >= 30 && rand < 40 && !parent.name.EndsWith(GameManager.birdNum.ToString()))
        {
            animator.SetTrigger("Jump");
            nextchange = Time.time + changerate;
        }
        else if (rand >= 40 && rand < 43 && !parent.name.EndsWith(GameManager.birdNum.ToString()))
        {
            animator.SetTrigger("RollAndJump");
            nextchange = Time.time + changerate;
        }
        else if (rand >= 43 && rand < 46 && !parent.name.EndsWith(GameManager.birdNum.ToString()))
        {
            animator.SetTrigger("RollAndJumpReverse");
            nextchange = Time.time + changerate;
        }
        else nextchange = Time.time + (changerate / 2);

    }
    
    void Cry()
    {
        int cryrand = Random.Range(0, 12);
        if (cryrand == 12) cryrand = 11;
        audioSource.PlayOneShot(crysound[cryrand]);
    }
}
