using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour {

    public Text levelNameText;      //关卡名称
    public Text highScoreText;      //最高分
    public Image bestStars;         //最高分对应星数
    public Sprite[] starImages;     //0-3星图片
    public GameObject greyWindow;   //灰色窗口

    [Space]                         //以下为胜利界面独有
    public Text scoreText;          //当前分数
    public Image stars;             //当前星数
    public Image newBestImage;      //新纪录图片
    public Button nextLevelButton;  //下一关按钮

    void OnEnable()
    {
        for(int i = 0; i < transform.parent.childCount; i++)            //将除了本身与灰色窗口以外的UI禁用
        {
            GameObject currentChild = transform.parent.GetChild(i).gameObject;
            if (currentChild != gameObject && currentChild != greyWindow) currentChild.SetActive(false);
        }

        Scene scene = SceneManager.GetActiveScene();
        string levelName = scene.name.PadLeft(5).Remove(0, 5);          //定义关卡名
        
        int highScore= PlayerPrefs.GetInt(levelName + "Highscore", 0);
        highScoreText.text = highScore.ToString();                          //读取最高分并应用
        levelNameText.text = levelName + " Fail!";
        int bestStarNum= PlayerPrefs.GetInt(levelName + "Star", 0);
        bestStars.sprite = starImages[bestStarNum];                         //读取最好星数并应用

        if (scoreText != null)  //关卡胜利时(有本次关卡分数)
        {
            levelNameText.text = levelName + " Complete!";
            scoreText.text = GameManager.score.ToString();
            stars.sprite = starImages[3 + GameManager.levelStar];           //改变星星图片
            if (GameManager.score > GameManager.lastHighScore) newBestImage.gameObject.SetActive(true);

            int levelNum = int.Parse(scene.name.PadLeft(7).Remove(0, 7));       //关卡编号
            if (levelNum < 3) nextLevelButton.gameObject.SetActive(true);       //若关卡后有下一关，则启用按钮
        }
    }
}
