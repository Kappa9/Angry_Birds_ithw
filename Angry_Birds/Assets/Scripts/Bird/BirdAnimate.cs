using UnityEngine;

public class BirdAnimate : MonoBehaviour {

    private Animator animator;
    private AudioSource audioSource;

    public AudioClip[] crysound;
    float nextchange, changerate;

    void Start() {
        changerate = Random.Range(0, 5);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.SetTrigger("Idle");                //初始化动画状态
        nextchange = Time.time + changerate;
    }
    
    void Update () {
        if(Time.time > nextchange)
        {
            if (transform.parent.name.EndsWith(GameManager.birdNum.ToString()) && (Birds.state == Birds.BirdState.Idle || Birds.state == Birds.BirdState.OnString))     //当前操作的鸟
            {
                changerate = Random.Range(2, 4);
                RandAnimation();
            }
            else if (!transform.parent.name.EndsWith(GameManager.birdNum.ToString()))       //其他鸟
            {
                changerate = Random.Range(2, 4);
                RandAnimation();
            }
        }
    }

    void RandAnimation()        //随机决定动画状态
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
        else if (rand >= 30 && rand < 40 && !transform.parent.name.EndsWith(GameManager.birdNum.ToString())) //当前操作的鸟无法变为此状态
        {
            animator.SetTrigger("Jump");
            nextchange = Time.time + changerate;
        }
        else if (rand >= 40 && rand < 43 && !transform.parent.name.EndsWith(GameManager.birdNum.ToString()))
        {
            animator.SetTrigger("RollAndJump");
            nextchange = Time.time + changerate;
        }
        else if (rand >= 43 && rand < 46 && !transform.parent.name.EndsWith(GameManager.birdNum.ToString()))
        {
            animator.SetTrigger("RollAndJumpReverse");
            nextchange = Time.time + changerate;
        }
        else nextchange = Time.time + (changerate / 2);     //若未改变，则间隔时间减半再重新决定

    }
    
    void Cry()      //通过AnimationEvent调用
    {
        int cryrand = Random.Range(0, 12);
        if (cryrand == 12) cryrand = 11;
        audioSource.PlayOneShot(crysound[cryrand]);
    }
}
