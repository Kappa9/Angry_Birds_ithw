using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {

    void OnEnable()
    {
        if (gameObject.name.StartsWith("MuteButton"))
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
    void Idle()
    {
        if(Time.timeScale!=0) GameManager.cursorInLevel = true;
        GameManager.cursorDrag = true;
    }

    public void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    public void LevelSelect()
    {
        Scene scene = SceneManager.GetActiveScene();
        int chapterNum = int.Parse(scene.name.PadLeft(12).Remove(0, 12));
        int levelNum = int.Parse(gameObject.name.PadLeft(11).Remove(0, 11));
        SceneManager.LoadScene("Level" + chapterNum + "-" + levelNum);
        Debug.Log("a"+chapterNum);
        Debug.Log(name);
        Debug.Log("b"+levelNum);
    }
    public void BackToSelect()
    {
        Scene scene = SceneManager.GetActiveScene();
        int chapterNum = int.Parse(scene.name.PadLeft(5).Remove(0, 5).PadRight(2).Remove(0, 2));
        SceneManager.LoadScene("LevelSelectE"+chapterNum);
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }
    public void TitleGoToSelect()
    {
        SceneManager.LoadScene("LevelSelectE1");
    }
    public void Mute()
    {
        GameObject mark = transform.GetChild(0).gameObject;
        if (AudioListener.volume != 0)
        {
            AudioListener.volume = 0;
            mark.SetActive(true);
        }
        else
        {
            AudioListener.volume = 1;
            mark.SetActive(false);
        }
    }
}
