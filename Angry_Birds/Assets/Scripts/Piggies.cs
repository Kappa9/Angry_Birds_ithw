using UnityEngine;

public class Piggies : MonoBehaviour {

    public float maxhp; //最大hp
    float hp, dmg = 0;  //当前hp，每次伤害的伤害值
    float dmgRate = 3f;   //伤害倍率

    private Animator animator;
    private AudioSource audioSource;
    public AudioClip[] OinkSound;   //猪叫声
    public AudioClip[] DeathSound;  //死亡声效（猪叫声）
    public AudioClip PopSound;      //死亡声效

    float nextchange, changerate;   //动画改变时间参数

    public GameObject pigScore;     //猪死亡后5000分图片
    public GameObject smokeBuff;    //猪死亡后生成的烟雾

    void Start () {
        changerate = Random.Range(0, 5);    //初始化动画改变频率
        hp = maxhp;     //初始化hp
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.SetTrigger("Idle");
        nextchange = Time.time + changerate;    //初始化动画状态与变化时间
        
    }

    void Update()
    {
        if (Time.time > nextchange) //时间足够
        {
            changerate = Random.Range(2, 4);    //再次改变频率
            RandAnimation();    //随机动画函数
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //定义相对速率与伤害计算
        float speed = collision.relativeVelocity.magnitude;
        if (speed > 1.5f) dmg = (speed - 1.5f) * dmgRate * 8f;
        else dmg = 0;
        hp -= dmg;
        if (hp <= 0)
        {
            GameManager.score += 5000;
            //烟雾与加分UI的生成与摧毁
            GameObject smoke= Instantiate(smokeBuff, transform.position, Quaternion.identity);
            Destroy(smoke, 0.45f);
            GameObject scoreUI=Instantiate(pigScore, transform.position + Vector3.up * 0.75f, Quaternion.identity);
            Destroy(scoreUI, 1f);
            GameManager.pigTotalNum--;      //猪总数-1

            int deathrand = Random.Range(0,8);
            if (deathrand == 8) deathrand = 7;
            audioSource.PlayOneShot(DeathSound[deathrand]);
            audioSource.PlayOneShot(PopSound);                  //播放各种声音

            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(gameObject, 1f);                    //摧毁游戏物体，1秒是为了防止动画提前结束
        }
        else        //根据hp百分比改变猪状态（从而改变动画）
        {
            if (hp >= maxhp * 2 / 3) 
            {
                animator.SetInteger("HpState", 0);
            }
            else if (hp >= maxhp * 1 / 3 && hp < maxhp * 2 / 3)
            {
                animator.SetInteger("HpState", 1);
            }
            else
            {
                animator.SetInteger("HpState", 2);
            }
        }
    }

    void RandAnimation()    //随机动画函数
    {
        int rand = Random.Range(0, 100);
        if (rand >= 0 && rand < 15)
        {
            animator.SetTrigger("Blink");
            nextchange = Time.time + changerate;
        }
        else if (rand >= 15 && rand < 30)
        {
            animator.SetTrigger("Smile");
            nextchange = Time.time + changerate;
        }
        else nextchange = Time.time + (changerate / 2);
        
    }
    void Oink() //猪叫（AnimationEvent中执行）
    {
        int cryrand = Random.Range(0, 10);
        if (cryrand == 10) cryrand = 9;
        audioSource.PlayOneShot(OinkSound[cryrand]);
    }
}
