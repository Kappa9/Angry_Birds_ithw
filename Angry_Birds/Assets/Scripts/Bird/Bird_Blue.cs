using UnityEngine;

public class Bird_Blue : MonoBehaviour {

    bool canUseAbility = true;   //是否能使用能力
    Rigidbody2D rb;
    ParticleSystem pr;

    AudioSource audioSource;
    public AudioClip[] abilitySound;
    public AudioClip[] collisionSound;
    
    public GameObject smoke_Ability;        //留在移动轨迹上的烟雾
    GameObject abilitySmoke;
    public GameObject smoke_Cloud;          //能力发动瞬间生成的烟雾

    void Start () {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        pr = GetComponent<ParticleSystem>();
        if (Birds.state == Birds.BirdState.Shot) canUseAbility = false;
    }
	
	void Update () {
        if (GameManager.cursorInLevel && gameObject.name.EndsWith(GameManager.birdNum.ToString()) && Input.GetMouseButtonDown(0) && Birds.state == Birds.BirdState.Shot && GetComponent<LineRenderer>()==null && canUseAbility && GameManager.gameState != GameManager.GameState.End)
        {
            canUseAbility = false;
            abilitySmoke = Instantiate(smoke_Ability, transform.position, Quaternion.identity);     //在运动轨迹上生成标记
            audioSource.PlayOneShot(abilitySound[0]);
            audioSource.PlayOneShot(abilitySound[1]);
            pr.Play();                              //播放声音与粒子效果
            GameObject smoke = Instantiate(smoke_Cloud, transform.position, Quaternion.identity);
            Destroy(smoke, 0.45f);                  //生成烟雾并延时摧毁

            Vector3 upperDirection = Vector2.Perpendicular(rb.velocity).normalized;

            GameManager.upperBlue = Instantiate(gameObject, transform.position + upperDirection * 0.3f, Quaternion.identity);
            GameManager.lowerBlue = Instantiate(gameObject, transform.position - upperDirection * 0.3f, Quaternion.identity);
            SplitBirds(GameManager.upperBlue,1);
            SplitBirds(GameManager.lowerBlue,-1);
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

    void SplitBirds(GameObject splitBird,float rate)
    {
        splitBird.GetComponentInChildren<Animator>().SetTrigger("Flying");
        Destroy(splitBird.GetComponent<LineRenderer>());
        splitBird.GetComponent<Rigidbody2D>().velocity = rb.velocity + Vector2.Perpendicular(rb.velocity).normalized * rate;  
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name.Contains((GameManager.birdNum - 1).ToString()))
        {
            canUseAbility = false;
            if (collision.relativeVelocity.magnitude >= 4f)
            {
                int birdCollsionRand = Random.Range(0, 5);
                if (birdCollsionRand == 5) birdCollsionRand = 0;
                audioSource.PlayOneShot(collisionSound[birdCollsionRand]);
            }
        }
    }
}
