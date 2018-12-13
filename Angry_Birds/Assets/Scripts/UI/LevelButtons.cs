using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtons : MonoBehaviour {

    public Text levelNumText;       //关卡数字文本
    public Sprite[] starSprites;    //0-3星图片
    public Image star;              //关卡星数
    public Image lockImage;         //关卡锁定
    
	void OnEnable () {
        levelNumText.text = gameObject.name.PadLeft(11).Remove(0, 11);  //初始化关卡数字文本

        Scene scene = SceneManager.GetActiveScene();
        int chapterNum = int.Parse(scene.name.PadLeft(12).Remove(0, 12));
        int levelNum = int.Parse(gameObject.name.PadLeft(11).Remove(0, 11));
        int bestStarNum = PlayerPrefs.GetInt(chapterNum + "-" + levelNum + "Star", 0);
        star.sprite = starSprites[bestStarNum];
        //设定关卡星数

        if (levelNum >= 2)      //通过上一关有无成绩决定是否锁定关卡
        {
            int starNumInLastLevel = PlayerPrefs.GetInt(chapterNum + "-" + (levelNum - 1) + "Star", 0);
            if (starNumInLastLevel == 0)
            {
                lockImage.gameObject.SetActive(true);
                GetComponent<Button>().interactable = false;
            }
        }
    }
}
