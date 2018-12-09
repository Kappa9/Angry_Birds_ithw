using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Blocks : MonoBehaviour {

    public enum BlockType       //方块种类
    {
        Wood,Galss,Stone
    }
    public BlockType blockType;
    AudioSource audioSource;
    ParticleSystem pr;

    public float maxhp;   //最大生命值
    float hp, dmg = 0;      //当前hp，每次伤害的伤害值
    float dmgRateOfRed = 1f, dmgRateOfBlue = 0.3f, dmgRateOfYellow = 1f;
    float dmgRateOfBlack = 1f, dmgRateOfWhite = 1f, dmgRateOfGreen = 1f;
    float dmgRateOfOthers = 2.5f;   //各项伤害倍率
    float dmgRate = 2.5f;   //当前攻击倍率

    public Text scorePlusText;  //加分文本
    public GameObject canvas;

    int soundRand1, soundRand2, soundRand3, soundRand4; //决定随机数范围的四个值

    [Tooltip("破损程度从0~3逐渐严重")]
    [SerializeField] Sprite[] BlockStates;          //图块状态
    [SerializeField] Sprite[] BreakParticles;       //破坏后生成粒子
    [Tooltip("0~11为木头，12~25为玻璃，26~36为石头")]
    [SerializeField] AudioClip[] DamageSounds;      //撞击声效

	void Start () {
        hp = maxhp;     //初始化hp
        audioSource = GetComponent<AudioSource>();
        pr = GetComponent<ParticleSystem>();

        switch (blockType)  //对图块类型分别进行操作
        {
            case BlockType.Wood:
                {
                    GetComponent<Rigidbody2D>().mass = 3;                               //设定质量
                    pr.textureSheetAnimation.SetSprite(0, BreakParticles[0]);
                    pr.textureSheetAnimation.SetSprite(1, BreakParticles[1]);
                    pr.textureSheetAnimation.SetSprite(2, BreakParticles[2]);
                    pr.textureSheetAnimation.SetSprite(3, null);
                    pr.textureSheetAnimation.SetSprite(4, null);                        //设定粒子
                    dmgRateOfRed = 2f; dmgRateOfYellow = 2.5f; dmgRateOfGreen = 2f;     //设定各项伤害倍率
                    soundRand1 = 0; soundRand2 = 3; soundRand3 = 6; soundRand4 = 12;    //限定随机数范围
                    break;
                }
            case BlockType.Galss:
                {
                    GetComponent<Rigidbody2D>().mass = 2;                               //同上
                    pr.textureSheetAnimation.SetSprite(0, BreakParticles[3]);
                    pr.textureSheetAnimation.SetSprite(1, BreakParticles[4]);
                    pr.textureSheetAnimation.SetSprite(2, BreakParticles[5]);
                    pr.textureSheetAnimation.SetSprite(3, BreakParticles[6]);
                    pr.textureSheetAnimation.SetSprite(4, BreakParticles[7]);
                    dmgRateOfBlue = 1.5f;
                    soundRand1 = 12; soundRand2 = 15; soundRand3 = 18; soundRand4 = 26;
                    break;
                }
            case BlockType.Stone:
                {
                    GetComponent<Rigidbody2D>().mass = 5;                               //同上
                    pr.textureSheetAnimation.SetSprite(0, BreakParticles[8]);
                    pr.textureSheetAnimation.SetSprite(1, BreakParticles[9]);
                    pr.textureSheetAnimation.SetSprite(2, BreakParticles[10]);
                    pr.textureSheetAnimation.SetSprite(3, null);
                    pr.textureSheetAnimation.SetSprite(4, null);
                    dmgRateOfRed = 0.75f; dmgRateOfYellow = 0.75f; dmgRateOfBlue = 0.15f;
                    dmgRateOfBlack = 3f; dmgRateOfWhite = 0.75f; dmgRateOfGreen = 0.75f;
                    soundRand1 = 26; soundRand2 = 29; soundRand3 = 32; soundRand4 = 37;
                    break;
                }
            default:break;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int soundRand, blockState;      //撞击声效随机数，图块状态

        if (collision.gameObject.tag == "Bird_Red") dmgRate = dmgRateOfRed;
        else if (collision.gameObject.tag == "Bird_Blue") dmgRate = dmgRateOfBlue;
        else if (collision.gameObject.tag == "Bird_Yellow") dmgRate = dmgRateOfYellow;
        else if (collision.gameObject.tag == "Bird_Black") dmgRate = dmgRateOfBlack;
        else if (collision.gameObject.tag == "Bird_White") dmgRate = dmgRateOfWhite;
        else if (collision.gameObject.tag == "Bird_Green") dmgRate = dmgRateOfGreen;
        else dmgRate = dmgRateOfOthers;     //根据被谁撞击决定伤害倍率
        float speed = collision.relativeVelocity.magnitude;     //相对速率
        if (speed > 4f) dmg = (speed - 4f) * dmgRate * 6f;         //伤害计算
        else dmg = 0;
        hp -= dmg;      //减血
        //Debug.Log(speed);
        //Debug.Log(dmg);

        if(collision.gameObject.layer == 10 && dmg >=1.5f)  //若被鸟撞击且伤害大于1.5（控制加分在10以上）
        {
            int birdAtkScore = (int)dmg * 7 - (int)dmg * 7 % 10;        //定义加分
            if (birdAtkScore > 500) birdAtkScore = 500;
            else if (birdAtkScore < 10) birdAtkScore = 10;      //控制加分
            ScorePlus(birdAtkScore);                            //执行加分函数
        }
        
        if (hp <= 0)        //若图块hp小于0
        {
            soundRand = Random.Range(soundRand2, soundRand3);
            if (soundRand == soundRand3) soundRand = soundRand3-1;
            audioSource.PlayOneShot(DamageSounds[soundRand]);   //播放摧毁音效

            pr.Play();      //生成粒子

            ScorePlus(500);     //分数增加500

            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(gameObject, 1f);                    //摧毁游戏物体，1秒是为了防止粒子动画提前结束
        }
        else
        {
            //根据伤害大小播放相应声效
            if (dmg < maxhp / 4 && dmg > maxhp / 16)
            {
                soundRand = Random.Range(soundRand3, soundRand4);
                if (soundRand == soundRand4) soundRand = soundRand-1;
                audioSource.PlayOneShot(DamageSounds[soundRand]);
            }
            else if(dmg >= maxhp / 4)
            {
                soundRand = Random.Range(soundRand1, soundRand2);
                if (soundRand == soundRand2) soundRand = soundRand2-1;
                audioSource.PlayOneShot(DamageSounds[soundRand]);
            }
            //根据hp百分比决定图块状态
            if (hp >= maxhp * 3 / 4) blockState = 0;
            else if (hp >= maxhp / 2 && hp < maxhp * 3 / 4) blockState = 1;
            else if (hp >= maxhp / 4 && hp < maxhp / 2) blockState = 2;
            else blockState = 3;
            GetComponent<SpriteRenderer>().sprite = BlockStates[blockState];    //根据状态改变渲染图片
        }
    }

    void ScorePlus(int plusScore)   //加分函数
    {
        GameManager.score += plusScore;
        //Vector3[] worldCs = new Vector3[4];
        //Vector3[] localCs = new Vector3[4];
        //canvas.GetComponent<RectTransform>().GetWorldCorners(worldCs);
        //float zoom = (localCs[1] - localCs[0]).y / (worldCs[1] - worldCs[0]).y;
        //Debug.Log(zoom);
        //Debug.Log(worldCs[0]);
        //canvas.GetComponent<RectTransform>().GetLocalCorners(localCs);
        //Vector3 viewPos = canvas.transform.InverseTransformPoint(transform.position + Vector3.up * 0.5f);

        //生成加分文字
        Text newText = Instantiate(scorePlusText, transform.position + Vector3.up * 0.1f, Quaternion.identity, canvas.transform);
        newText.text = plusScore.ToString();
    }
}
