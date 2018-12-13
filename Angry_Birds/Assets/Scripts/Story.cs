using UnityEngine;

public class Story : MonoBehaviour {

    float startTime, endPos=-5.17f;         //开始时间，终止位置

	void Start () {
        startTime = Time.time + 4f;         //4秒后画面开始移动
	}
	
	void Update () {
        if (Time.time >= startTime && transform.position.x>=endPos)     //开始移动后，移动到终止位置前
        {
            transform.Translate(Vector2.left * Time.unscaledDeltaTime * 1.15f);
        }
	}
}
