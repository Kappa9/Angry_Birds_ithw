using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour {

    public AudioClip[] BGM;
    public static BGMManager instance = null;
    AudioSource au;

	void Awake () {
        DontDestroyOnLoad(gameObject);          //不自动摧毁
        if (instance == null)                   //单例模式
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        au = GetComponent<AudioSource>();
        float gameVolume = PlayerPrefs.GetFloat("Volume", 1);
        AudioListener.volume = gameVolume;
    }
	
	void Update () {
        if (AudioListener.volume != 0)  //只有非静音时才开始播放
        {
            if (SceneManager.GetActiveScene().name == "Story")      //当前为故事场景时
            {
                if (au.clip != BGM[2])
                {
                    au.clip = BGM[2];
                    au.loop = false;
                    au.Play();
                }
                if (!au.isPlaying || Input.GetMouseButtonDown(0)) SceneManager.LoadScene("Level1-1");   //故事场景中跳转到关卡的方式
            }
            else
            {
                if (!au.isPlaying) au.Play();
                au.loop = true;
                if (GameManager.typeOfSceneNum == 1 && au.clip != BGM[1])   //关卡中
                {
                    au.clip = BGM[1]; au.Play();
                }
                else if (GameManager.typeOfSceneNum == 0 && au.clip != BGM[0])  //关卡外
                {
                    au.clip = BGM[0]; au.Play();
                }
            }
        }
        else au.Stop();     //静音即停止
    }
}
