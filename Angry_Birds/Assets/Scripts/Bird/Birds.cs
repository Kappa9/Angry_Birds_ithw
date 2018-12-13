using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birds : MonoBehaviour {

    public enum BirdState   //鸟状态（公共）
    {
        Idle, OnString, Grabbed, Shot
    }
    public static BirdState state;
    Rigidbody2D rb;
    LineRenderer lr;
    TrailRenderer tr;
    ParticleSystem pr;
    CapsuleCollider2D co;

    [Range(1, 10)] public float moveSpeed;  //鸟被抛射的速度

    AudioSource audioSource;
    public AudioClip[] slingShotSound;      //被抛射后声音
    public AudioClip[] nextMilitarySound;   //换鸟音效
    public AudioClip destorySound;          //鸟消失音效
    public GameObject smokeBuff;            //碰撞或消失生成的烟雾
    public Transform scoreTextParent;       //加分文字的父物体

    [Space(10)]
    public AudioClip selectSound;           //鸟被选中音效
    public AudioClip flyingSound;           //鸟被抛射音效
    public Sprite[] fertherParticles;       //碰撞或消失生成的粒子
    
    Vector2 startColliderSize;              //初始碰撞体积（储存用）

    bool oncePerformed = false, toSlingOncePerformed = false;//是否执行操作参数
    bool collided = false;                  //是否被撞
    float timeToDie = 0;                    //被撞后执行摧毁函数时间

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        tr = GetComponent<TrailRenderer>();
        pr = GetComponent<ParticleSystem>();
        co = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();

        if (state == BirdState.Idle)
        {
            tr.emitting = false;                //设定不产生轨迹
            rb.bodyType = RigidbodyType2D.Kinematic;  //改变物理类型
            startColliderSize = co.size;        //储存碰撞体体积
        }
    }

    void Update()
    {
        if (gameObject.name.EndsWith(GameManager.birdNum.ToString()) && state == BirdState.Idle && rb != null)  //鸟状态为闲置时
        {
            if (!toSlingOncePerformed) //未执行操作时（以下只执行一次）
            {
                GetComponentInChildren<Animator>().SetTrigger("Shot");  //改变动画状态，以执行旋转动作
                rb.bodyType = RigidbodyType2D.Dynamic;  //改变物理类型
                rb.gravityScale = 0;    //重力清零
                if(GameManager.birdNum!=1 && GameManager.gameState != GameManager.GameState.End)
                {
                    int nextMilitaryRand = Random.Range(0, 3);
                    if (nextMilitaryRand == 3) nextMilitaryRand = 0;
                    audioSource.PlayOneShot(nextMilitarySound[nextMilitaryRand]);   //播放鸟上场声音
                }
                toSlingOncePerformed = true;
            }
            rb.position = Vector3.MoveTowards(transform.position, GameManager.midAnchorPos, Time.deltaTime * 4);
            //朝向弹弓中心移动
            if (rb.position == (Vector2)GameManager.midAnchorPos)   //移动完成条件
            {
                GetComponentInChildren<Animator>().SetTrigger("Idle");  //改变动画状态
                co.size = new Vector2(0.8f, 0.75f);     //增大碰撞体体积以方便拖拽
                state = BirdState.OnString;     //改变鸟状态
                rb.freezeRotation = true;       //阻止鸟旋转
            }
        }
        if (transform.position.x < -15 || transform.position.x > 30) DestoryObject();   //超过一定范围后执行摧毁函数
        if (collided && !oncePerformed && GameManager.gameState != GameManager.GameState.End)   //撞击过后待摧毁过程
        {
            if (rb!=null && rb.velocity.magnitude <= 1) 
            {
                timeToDie += Time.deltaTime; //速度小于1时增加时间
                if (timeToDie >= 4f)
                {
                    oncePerformed = true;
                    DestoryObject();        //执行摧毁函数
                }
            }
        }
        if (GameManager.gameState == GameManager.GameState.End) Destroy(GetComponent<TrailRenderer>()); //若关卡结束，摧毁轨迹
    }

    void OnMouseDown()
    {
        if (gameObject.name.EndsWith(GameManager.birdNum.ToString()) && state == BirdState.OnString && GameManager.gameState != GameManager.GameState.End && Time.timeScale != 0)  //鸟被拖拽的条件（第一个条件为名字的比较）
        {
            co.size = startColliderSize;    //重置碰撞体体积
            lr.enabled = true;              //激活预测轨迹
            audioSource.PlayOneShot(selectSound);
            state = BirdState.Grabbed;      //改变鸟状态
        }
    }

    void OnMouseDrag()
    {
        if (gameObject.name.EndsWith(GameManager.birdNum.ToString()) && state == BirdState.Grabbed && Time.timeScale != 0)
        {
            if (GameManager.gameState != GameManager.GameState.End)
            {
                rb.position = SlingString.birdPos;  //拖拽时调整鸟的位置
                var points = new Vector3[20];       //定义坐标数组
                for (int i = 0; i < 20; i++)         //计算预测轨迹的20个点
                {
                    points[i] = transform.position + new Vector3(-SlingString.delta.x * moveSpeed * i * 0.1f, -SlingString.delta.y * moveSpeed * i * 0.1f - 5f * i * i * 0.01f / 2, 0);
                }
                lr.SetPositions(points);            //设定预测轨迹
            }
            else
            {
                lr.enabled = false;
                rb.position = SlingString.birdPos;
            }
        }
        
    }

    void OnMouseUp()
    {
        if (gameObject.name.EndsWith(GameManager.birdNum.ToString()) && state == BirdState.Grabbed && GameManager.gameState != GameManager.GameState.End && Time.timeScale != 0) //松开鼠标执行以下内容的条件
        {
            if (SlingString.delta.x * SlingString.delta.x + SlingString.delta.y * SlingString.delta.y < 0.16) //弹弓与鸟位置之差小于一定范围时
            {
                lr.enabled = false;         //暂时禁用预测轨迹
                transform.position = SlingString.middleAnchorStartPos;  //复位
                co.size = new Vector2(0.8f, 0.75f); //复位
                state = BirdState.OnString;     //复位至未拖拽状态
            }
            else
            {
                int shotRand = Random.Range(0, 3);
                if (shotRand == 3) shotRand = 2;
                audioSource.PlayOneShot(slingShotSound[shotRand]);
                audioSource.PlayOneShot(flyingSound);                   //播放弹弓抛射声与鸟飞行声

                rb.position = SlingString.middleAnchorStartPos + new Vector3(0.05f, 0.05f, 0);  //改变鸟的位置
                rb.velocity = SlingString.delta * new Vector2(-1, -1) * moveSpeed;  //施加反向速度
                rb.freezeRotation = false;  //接触旋转限制
                rb.gravityScale = 1;     //复位重力

                Destroy(lr);
                tr.emitting = true;         //允许生成轨迹
                state = BirdState.Shot;     //改变鸟状态
                GameManager.thisBird = gameObject;      //记录正在被操作的鸟
                GetComponentInChildren<Animator>().SetTrigger("Flying");
                GameManager.birdTotalNum--;     //余下鸟数量-1
                GameManager.canDefineResult = false;    //无法判断游戏结果
            }
        }
    }

    void DestoryObject()
    {
        if (GetComponent<Rigidbody2D>() != null)    //若能检测到物理组件（使以下代码只能执行一次）
        {
            GameObject child = transform.GetChild(0).gameObject;
            child.SetActive(false);                 //隐藏子物体
            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Rigidbody2D>());   //摧毁碰撞体与物理组件
            audioSource.PlayOneShot(destorySound);
            pr.Play();                              //播放声音与粒子效果
            GameObject smoke = Instantiate(smokeBuff, transform.position, Quaternion.identity);
            Destroy(smoke, 0.45f);                  //生成烟雾并延时摧毁
            Invoke("TryDefination", 0.75f);         //0.75秒后能够判断游戏结果
            GameManager.camOncePerformed = false;
            GameManager.camNextChange = Time.time + GameManager.camChangeRate;      //0.8秒后使相机移动
            if (!collided && tr!=null && tr.emitting == true)   //如果并未撞击就被摧毁
            {
                tr.emitting = false;        //禁止轨迹
                if (GameManager.birdNum >= 2 && GameManager.lastBird.GetComponent<TrailRenderer>() != null)
                {
                    Destroy(GameManager.lastBird.GetComponent<TrailRenderer>());  //摧毁上一只鸟的轨迹
                    if (GameManager.lastBird.tag == "Bird_Blue")
                    {
                        if(GameManager.upperBlue != null) Destroy(GameManager.upperBlue.GetComponent<TrailRenderer>());
                        if(GameManager.lowerBlue != null) Destroy(GameManager.lowerBlue.GetComponent<TrailRenderer>());
                    }
                }
                collided = true;
                oncePerformed = false;
                if (gameObject.name.EndsWith(GameManager.birdNum.ToString()) && GameManager.birdTotalNum > 0)
                {
                    GameManager.lastBird = gameObject;      //重新设定上一只鸟
                    GameManager.birdNum++;      //鸟的顺位+1
                    if (GameManager.gameState != GameManager.GameState.End) Invoke("NextBird", 1.5f);   //1.5秒后下一只鸟上场
                }
                else if (GameManager.birdTotalNum <= 0) state = BirdState.Idle;
            }
        }
        if (GetComponent<TrailRenderer>() == null) gameObject.SetActive(false);  //若该鸟轨迹已被摧毁，则禁用游戏物体
    }

    void TryDefination()    //0.75秒后能够判断游戏结果
    {
        if(state!=BirdState.Shot) GameManager.canDefineResult = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name.Contains(GameManager.birdNum.ToString()) && state == BirdState.Shot)
        {
            if (collision.relativeVelocity.magnitude >= 4f) //若相对速率大于4
            {
                pr.Play();
                GameObject smoke = Instantiate(smokeBuff, transform.position, Quaternion.identity);
                Destroy(smoke, 0.45f);
            }
            if (tr!=null && tr.emitting == true)
            {
                tr.emitting = false;        //禁止轨迹
                if (GameManager.birdNum >= 2 && GameManager.lastBird.GetComponent<TrailRenderer>() != null)
                {
                    Destroy(GameManager.lastBird.GetComponent<TrailRenderer>());  //摧毁上一只鸟的轨迹
                    if (GameManager.lastBird.GetComponent<Rigidbody2D>() == null) GameManager.lastBird.SetActive(false);//若已失去物理组件，则禁用
                    if (GameManager.lastBird.tag == "Bird_Blue")
                    {
                        if (GameManager.upperBlue != null)
                        {
                            Destroy(GameManager.upperBlue.GetComponent<TrailRenderer>());
                            if (GameManager.upperBlue.GetComponent<Rigidbody2D>() == null) Destroy(GameManager.upperBlue);
                        }
                        if (GameManager.lowerBlue != null)
                        {
                            Destroy(GameManager.lowerBlue.GetComponent<TrailRenderer>());
                            if (GameManager.lowerBlue.GetComponent<Rigidbody2D>() == null) Destroy(GameManager.lowerBlue);
                        }
                    }
                }
                collided = true;
                oncePerformed = false;
                GetComponentInChildren<Animator>().SetTrigger("Collision");
                if (gameObject.name.EndsWith(GameManager.birdNum.ToString()))
                {
                    GameManager.lastBird = gameObject;      //重新设定上一只鸟
                    GameManager.birdNum++;      //鸟的顺位+1
                    if (GameManager.gameState != GameManager.GameState.End) Invoke("NextBird", 1.5f);   //1.5秒后下一只鸟上场
                }
            }
        }
        if (gameObject.name.Contains((GameManager.birdNum - 1).ToString()))
        {
            if (collision.relativeVelocity.magnitude >= 4f) //若相对速率大于4
            {
                pr.Play();
                GameObject smoke = Instantiate(smokeBuff, transform.position, Quaternion.identity);
                Destroy(smoke, 0.45f);
            }
            if (GetComponent<TrailRenderer>()!=null && tr.emitting == true)
            {
                tr.emitting = false;        //禁止轨迹
                collided = true;
                oncePerformed = false;
                GetComponentInChildren<Animator>().SetTrigger("Collision");
            }
        }
    }

    void NextBird()     //1.5秒后下一只鸟上场
    {
        state = BirdState.Idle;     //改变鸟状态
    }

}
