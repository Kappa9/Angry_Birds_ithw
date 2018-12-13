using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {

    void OnEnable()
    {
        if (gameObject.name.StartsWith("MuteButton"))       //静音按钮启用时检查游戏是否为静音
        {
            GameObject mark = transform.GetChild(0).gameObject;
            if (AudioListener.volume == 0) mark.SetActive(true);
            else mark.SetActive(false);
        }
    }

    void HighLighted()
    {
        GameManager.cursorInLevel = false;
        GameManager.cursorDrag = true;
    }
    void Idle()     //按钮处于特定状态时改变鼠标状态
    {
        if(Time.timeScale!=0) GameManager.cursorInLevel = true;
        GameManager.cursorDrag = true;
    }

    public void RestartLevel()  //重开
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    public void LevelSelect()   //选择关卡
    {
        Scene scene = SceneManager.GetActiveScene();
        int chapterNum = int.Parse(scene.name.PadLeft(12).Remove(0, 12));
        int levelNum = int.Parse(gameObject.name.PadLeft(11).Remove(0, 11));
        if (chapterNum != 1 || levelNum != 1) SceneManager.LoadScene("Level" + chapterNum + "-" + levelNum);
        else SceneManager.LoadScene("Story");
        
    }
    public void GoToNextLevel()     //进入下一关
    {
        Scene scene = SceneManager.GetActiveScene();
        int chapterNum = int.Parse(scene.name.PadLeft(5).Remove(0, 5).PadRight(2).Remove(1, 2));
        int levelNum = int.Parse(scene.name.PadLeft(7).Remove(0, 7));
        if (levelNum < 5) SceneManager.LoadScene("Level" + chapterNum + "-" + (levelNum + 1));
    }
    public void BackToSelect()      //回到选关
    {
        Scene scene = SceneManager.GetActiveScene();
        int chapterNum = int.Parse(scene.name.PadLeft(5).Remove(0, 5).PadRight(2).Remove(1, 2));
        SceneManager.LoadScene("LevelSelectE"+chapterNum);
    }
    public void BackToTitle()       //回到标题
    {
        SceneManager.LoadScene("TitleScreen");
    }
    public void TitleGoToSelect()   //从标题进入选关
    {
        SceneManager.LoadScene("LevelSelectE1");
    }
    public void Mute()      //设定(取消)静音
    {
        GameObject mark = transform.GetChild(0).gameObject;
        if (AudioListener.volume != 0)
        {
            AudioListener.volume = 0;
            PlayerPrefs.SetFloat("Volume", 0);
            mark.SetActive(true);
        }
        else
        {
            AudioListener.volume = 1;
            PlayerPrefs.SetFloat("Volume", 1);
            mark.SetActive(false);
        }
    }
}
