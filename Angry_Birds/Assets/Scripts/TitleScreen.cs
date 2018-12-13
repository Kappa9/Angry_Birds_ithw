using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour {

    public GameObject randomBird;       //随机生成的鸟的原物体
    public Sprite[] randomBirdSprites;
    float nextChange, changeRate;

    public GameObject sky;
    public GameObject ground;

	void Start () {
        changeRate = Random.Range(0.75f, 1.5f);
        nextChange = Time.time + changeRate;            //调整随机生成鸟的时间
        
	}
	
	void Update () {
        if (Time.time >= nextChange)        //一段时间后生成鸟
        {
            SpawnRandomBird();
            changeRate = Random.Range(0.25f, 1.5f);
            nextChange = Time.time + changeRate;            //调整随机生成鸟的时间
        }
        sky.transform.Translate(Vector2.left * Time.unscaledDeltaTime * 1f);
        if (sky.transform.position.x <= -13.7465f) sky.transform.position = new Vector3(0, -2.95f, 0);
        ground.transform.Translate(Vector2.left * Time.unscaledDeltaTime * 1.2f);
        if (ground.transform.position.x <= -23.67f) ground.transform.position = Vector3.zero;
    }

    void SpawnRandomBird()
    {
        Vector3 randBirdPos = new Vector3(Random.Range(-11.5f, 7f),-3.4f, 0);              //设置随机生成坐标
        GameObject spawnedBird = Instantiate(randomBird, randBirdPos, Quaternion.identity, transform);      //随机生成鸟

        spawnedBird.transform.localScale = Vector3.one * Random.Range(0.8f, 1.8f);           //设置随机大小

        int spriteRand = Random.Range(0, 3);                                                 //设置随机图片
        if (spriteRand == 3) spriteRand = 0;
        spawnedBird.GetComponent<SpriteRenderer>().sprite = randomBirdSprites[spriteRand];

        Vector2 randVelocity = new Vector2(Random.Range(5f, 9f), Random.Range(5f, 9f));     //设置随机速度
        spawnedBird.GetComponent<Rigidbody2D>().velocity = randVelocity;
        if (spawnedBird.transform.position.x >= 10 || spawnedBird.transform.position.y <= -3.65f) Destroy(spawnedBird);

        Destroy(spawnedBird, 5f);           //延时摧毁
    }
}
