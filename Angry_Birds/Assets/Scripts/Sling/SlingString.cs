using UnityEngine;

public class SlingString : MonoBehaviour {

    public Transform anchor,middleAnchor;       //弹弓锚点，弹弓中心点
    public static Vector3 middleAnchorStartPos, delta, birdPos;     //弹弓中心点位置，鸟位置与其差值，鸟在弹弓上位置
    bool posUpdated=false;

	void Start () {
        middleAnchorStartPos = middleAnchor.position;       //储存开始关卡时中心点位置
        birdPos = middleAnchor.position;    //初始化鸟在弹弓上的的位置
    }


    void FixedUpdate()
    {
        if (Birds.state == Birds.BirdState.Grabbed && GameManager.gameState != GameManager.GameState.End)
        {
            posUpdated = true;
            UpdatePosition();   //执行位置更新函数
        }
        else if(posUpdated)
        {
            //重置所有
            transform.localPosition = anchor.localPosition;
            transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
            transform.localScale = Vector3.one;
            birdPos = middleAnchor.position;
            posUpdated = false;
        }
	}

    void UpdatePosition()       //位置更新函数
    {
        delta = GameManager.GetWorldMousePos() - middleAnchor.position;     //设定初始差值为鼠标到中心点间距离
        if (((Vector2)delta).magnitude > 1.5f)       //差值超出半径
        {
            delta = delta * 1.5f / ((Vector2)delta).magnitude;
        }
        Vector3 endPos = middleAnchor.position+ anchor.position + delta;        //与绳子最终位置相关              
        birdPos = middleAnchor.position + delta;

        transform.position = new Vector3((endPos.x) / 2f,(endPos.y) / 2f, transform.position.z);        //设定绳子最终位置
        transform.rotation = Quaternion.Euler(180f, 0f,Vector2.SignedAngle(delta, Vector2.right));      //设定绳子旋转
        float scalex = ((Vector2)anchor.position - (Vector2)(birdPos)).magnitude;
        float scaley = 2 / scalex;
        if (scaley > 1) scaley = 1;
        transform.localScale = new Vector3(scalex*13, scaley, 1);               //设置绳子缩放
    }
}
