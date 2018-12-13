using UnityEngine;

public class Bird_Yellow : MonoBehaviour {

    bool canUseAbility = true;   //是否能使用能力
    Rigidbody2D rb;
    ParticleSystem pr;

    public AnimationClip boostAnimation;
    AnimationClip currentClip;

    AudioSource audioSource;
    public AudioClip abilitySound;
    public AudioClip[] collisionSound;

    public GameObject smoke_Ability;
    GameObject abilitySmoke;
    public GameObject smoke_Cloud;
    
    
    void Start () {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        pr = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (GameManager.cursorInLevel && gameObject.name.EndsWith(GameManager.birdNum.ToString()) && Input.GetMouseButtonDown(0) && Birds.state == Birds.BirdState.Shot && GetComponent<LineRenderer>() == null && canUseAbility && GameManager.gameState != GameManager.GameState.End)       //各种条件
        {
            canUseAbility = false;
            abilitySmoke = Instantiate(smoke_Ability, transform.position, Quaternion.identity);         //在运动轨迹上生成标记
            audioSource.PlayOneShot(abilitySound);
            pr.Play();                              //播放声音与粒子效果
            GameObject smoke = Instantiate(smoke_Cloud, transform.position, Quaternion.identity);
            Destroy(smoke, 0.45f);                  //生成烟雾并延时摧毁

            rb.velocity = rb.velocity.normalized * 20f;     //加速
        }
        if (!canUseAbility && GetComponent<TrailRenderer>() == null)
        {
            Destroy(abilitySmoke);
        }
    }

    void OnDisable()
    {
        Destroy(abilitySmoke);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name.Contains((GameManager.birdNum-1).ToString()))//撞击后无法发动能力条件
        {
            canUseAbility = false;      //撞击后无法发动能力条件
            if (collision.relativeVelocity.magnitude >= 4f)       //发出叫声
            {
                int birdCollsionRand = Random.Range(0, 5);
                if (birdCollsionRand == 5) birdCollsionRand = 0;
                audioSource.PlayOneShot(collisionSound[birdCollsionRand]);
            }
        }
    }
}
