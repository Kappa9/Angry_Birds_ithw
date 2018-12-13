using UnityEngine;

public class SlingHolder : MonoBehaviour {

    Vector3 startPos;   //开始关卡时位置，储存用
    SpriteRenderer sr;
    Rigidbody2D rb;
    AudioSource audioSource;

    void Start () {
        startPos = transform.position;      //储存开始关卡时弹弓托位置
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }
	
	void FixedUpdate () {
        if (Birds.state == Birds.BirdState.Grabbed && GameManager.gameState != GameManager.GameState.End)   //鸟被拖拽时
        {
            rb.position = SlingString.birdPos;  //让其与鸟的位置一致(因为设定了中心点所以不会重合)
            transform.rotation = Quaternion.Euler(180f, 0f, Vector2.SignedAngle(SlingString.delta, Vector2.up)+75); //合适旋转
            transform.localScale = new Vector3(1.5f, 1.5f, 0);
            sr.sortingOrder = 3;                                //将托变大并改变层级，使托变明显
        }
        else
        {
            rb.position = startPos;             //重置位置，旋转，大小，层级
            transform.rotation = Quaternion.Euler(180, 0, 75);
            transform.localScale = new Vector3(1, 1, 0);
            sr.sortingOrder = 0;
        }
        if (Birds.state == Birds.BirdState.Grabbed && GameManager.gameState != GameManager.GameState.End)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();     //鸟被拖拽时发出声音(组件中已设定循环)
            }
        }
        else audioSource.Stop();    //否则停止声音
    }
}
