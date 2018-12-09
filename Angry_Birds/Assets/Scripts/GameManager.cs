﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static bool cursorInLevel, cursorDrag, canDefineResult, camOncePerformed;    //鼠标指针参数，结果判断参数，相机是否执行参数
    public enum TypeOfScene //场景类型
    {
        TitleScreen,LevelSelect,LevelPlaying
    }
    public TypeOfScene typeOfScene;
    public enum GameState   //关卡状态
    {
        Start,Playing,End
    }
    public static GameState gameState;

    public GameObject gameCamera;   //相机
    public GameObject GreyPanel;    //灰色画板（暂停时外部的灰色遮罩）
    public GameObject PauseMenu;    //暂停菜单
    public GameObject ExitWindow;   //游戏退出窗口
    public GameObject ResultWindow; //关卡结算窗口

    public Transform middleAnchor;  //弹弓中间点
    public static Vector3 midAnchorPos; //弹弓中间点位置（存储用）
    public Transform sling;         //弹弓（父物体）（坐标决定相机自动移动的限度）
    public Transform birdsParent;   //所有鸟的公共父物体
    public Transform piggiesParent; //所有猪的公共父物体（坐标决定相机初始位置）
    public GameObject sky;          //天空

    AudioSource audioSource;
    public AudioClip[] music;           //BGM
    public AudioClip[] levelPlaying;    //与关卡状态有关的音效
    
    [Header("Cursor Settings")]         //标题属性（在编辑器中查看）
    public Texture2D[] cursorTexture;   //鼠标指针材质
    public CursorMode cursorMode;       //鼠标指针模式
    public Vector2 hotSpot;             //鼠标指针“热点”
    float mousepos, lastmousepos;       //这一帧与上一帧的鼠标指针位置



    public static int score, birdNum, birdTotalNum, pigTotalNum;  //积分，鸟的出场顺位，鸟（空闲状态）和猪的当前个数
    public static GameObject lastBird, thisBird; //上一只鸟，当前被操作的鸟

    float scoreInText = 0, nextChange, changeRate = 0.1f;   //UI积分，积分改变时间参数
    public static float camNextChange, camChangeRate = 0.8f;    //相机移动时间参数
    bool oncePerformed = false; //是否执行操作参数

    public Text scoreText;      //当前积分
    public Text highScoreText;  //最高分
    public Text levelNameText;  //暂停窗口中的关卡名字

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;

        if (typeOfScene==TypeOfScene.LevelPlaying)      //场景状态为关卡时
        {
            score = 0;
            Scene scene = SceneManager.GetActiveScene();
            levelNameText.text = scene.name.PadLeft(5).Remove(0, 5);//显示关卡名字

            canDefineResult = true;
            gameState = GameState.Start;
            camOncePerformed = true;                    //各种初始化
            gameCamera.transform.position = new Vector3(piggiesParent.position.x, 0, -10);      //使相机对准敌方区域中心位置（预先设定）
            midAnchorPos = middleAnchor.position;       //储存弹弓中间点的坐标

            birdNum = 1;
            lastBird = null;
            thisBird = null;                            //各种初始化
            birdTotalNum = birdsParent.childCount;      
            pigTotalNum = piggiesParent.childCount;     //初始化鸟与猪的个数
            
            audioSource.clip = music[1];
            audioSource.Play();                         //切换音乐
            int levelStartrand = Random.Range(0, 2);
            if (levelStartrand == 2) levelStartrand = 0;
            audioSource.PlayOneShot(levelPlaying[levelStartrand]);      //随机播放关卡开始音效

            cursorInLevel = true;
            cursorDrag = true;                          
            Cursor.SetCursor(cursorTexture[0], hotSpot, cursorMode);    //初始化鼠标状态
            mousepos = Input.mousePosition.x;           //记录开始时鼠标位置
        }
        else
        {
            audioSource.clip = music[0];
            audioSource.Play();                         //切换音乐

            Cursor.SetCursor(cursorTexture[0], hotSpot, cursorMode);    //初始化鼠标状态

            if (typeOfScene == TypeOfScene.TitleScreen)
            {
                
            }
            else
            {

            }
        }  
    }
    
    void Update()
    {
        if (typeOfScene == TypeOfScene.LevelPlaying)    //场景类型为关卡时
        {
            if (gameState == GameState.Start)
            {
                Invoke("CameraAutoManagement", 0.75f);
            }
            else CameraAutoManagement();

            if (Input.GetKeyDown(KeyCode.R))        //按下R键重开
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

            lastmousepos = mousepos;
            mousepos = Input.mousePosition.x;       //分别记录上一帧与这一帧鼠标位置来作差

            if (Time.timeScale == 1 &&  gameState != GameState.End)     //能手动调整相机的条件
            {
                CameraManualManagement();
            }
            if (canDefineResult && gameState != GameState.End && Birds.state!=Birds.BirdState.Shot)     //能判断关卡结果的条件
            {
                LevelResult();
            }
            ScoreManagement();      //积分UI管理
        }

        CursorManagement();     //鼠标管理
    }

    

    void CameraManualManagement()       //相机手动调节
    {
        float scrspeed = Input.GetAxis("Mouse ScrollWheel");
        float speedSign = 0;
        if (scrspeed != 0) speedSign = scrspeed / System.Math.Abs(scrspeed);

        if (gameCamera.GetComponent<Camera>().orthographicSize <= 6.5 && gameCamera.GetComponent<Camera>().orthographicSize >= 4)
        {
            gameCamera.GetComponent<Camera>().orthographicSize += speedSign * Time.unscaledDeltaTime * 10;      //相机缩放
            sky.transform.localScale += new Vector3(1, 1, 0) * speedSign * Time.unscaledDeltaTime * 10 / 7f;    //天空缩放
        }
        else if (gameCamera.GetComponent<Camera>().orthographicSize > 6.5 && scrspeed < 0)
        {
            gameCamera.GetComponent<Camera>().orthographicSize += speedSign * Time.unscaledDeltaTime * 10;
            sky.transform.localScale += new Vector3(1, 1, 0) * speedSign * Time.unscaledDeltaTime * 10 / 7f;
        }
        else if (gameCamera.GetComponent<Camera>().orthographicSize < 4 && scrspeed > 0)
        {
            gameCamera.GetComponent<Camera>().orthographicSize += speedSign * Time.unscaledDeltaTime * 10;
            sky.transform.localScale += new Vector3(1, 1, 0) * speedSign * Time.unscaledDeltaTime * 10 / 7f;
        }

        if(Input.GetMouseButton(0) && lastmousepos - mousepos != 0 && Birds.state != Birds.BirdState.Grabbed && Birds.state != Birds.BirdState.Shot)        //能拖动鼠标移动视角的条件
        {
            gameCamera.GetComponent<Rigidbody2D>().velocity = new Vector2(lastmousepos - mousepos, 0);      //给予相机速度
        }
    }

    void CameraAutoManagement()         //相机自动调整
    {
        if (gameState == GameState.Start)       //关卡状态为开始时，延时0.75秒被调用
        {
            gameCamera.transform.Translate(Vector3.left * Time.unscaledDeltaTime * 8f);     //移动相机
            if (gameCamera.transform.position.x <= sling.position.x)gameState = GameState.Playing;  //移动到合适坐标后改变关卡状态为进行中
        }
        else if (gameState == GameState.Playing)    //关卡状态为进行中
        {
            if (Birds.state == Birds.BirdState.Shot)    //鸟状态为飞行时（从被抛射到下一只鸟上弹弓的一段时间）
            {
                if(thisBird.transform.position.x >= GetViewPos(new Vector3(0.33f, 0, 0)).x)     //鸟视觉坐标超过相机左三分之一部分时
                {
                    gameCamera.GetComponent<Rigidbody2D>().velocity = Vector2.right * 6f;       //移动相机
                }

                if(thisBird.transform.position.y>=GetViewPos(new Vector3(0,0.8f,0)).y && gameCamera.GetComponent<Camera>().orthographicSize <= 6.5)     //鸟视觉坐标超过相机下部80%且相机大小小于6.5时
                {
                    gameCamera.GetComponent<Camera>().orthographicSize += Time.unscaledDeltaTime * 2;   
                    sky.transform.localScale += new Vector3(1, 1, 0) * Time.unscaledDeltaTime * 2 / 7f; //缩放相机与天空
                }
                else if (thisBird.transform.position.y <= GetViewPos(new Vector3(0, 0.7f, 0)).y && gameCamera.GetComponent<Camera>().orthographicSize >= 5.5)   //鸟视觉坐标未达到相机下部70%且相机大小大于5.5时
                {
                    gameCamera.GetComponent<Camera>().orthographicSize -= Time.unscaledDeltaTime * 3;
                    sky.transform.localScale -= new Vector3(1, 1, 0) * Time.unscaledDeltaTime * 3 / 7f; ////缩放相机与天空
                }
            }
            else    //鸟的其他状态
            {
                if (!camOncePerformed && Time.time >= camNextChange)    //未执行操作且时间条件满足时（条件设定在Birds类中）
                {
                    gameCamera.transform.Translate(Vector3.left * Time.unscaledDeltaTime * 8f); //移动相机
                    if (gameCamera.transform.position.x <= sling.position.x) camOncePerformed = true;   //移动到合适坐标后确认相机已执行操作
                }
            }
        }
        else        //关卡状态为结束时
        {
            if (!camOncePerformed)      //未执行操作时（条件设定在Birds类中）
            {
                gameCamera.transform.Translate(Vector3.left * Time.unscaledDeltaTime * 20f);    //移动相机
                if (gameCamera.transform.position.x <= sling.position.x) camOncePerformed = true;   //移动到合适坐标后确认相机已执行操作
            }
        }
    }

    void CursorManagement() //鼠标指针管理
    {
        if (typeOfScene!=TypeOfScene.LevelPlaying || !cursorInLevel || Time.timeScale==0)   //场景类型不为关卡或鼠标在按钮UI上或游戏暂停时
        {
            if (Input.GetMouseButton(0) && cursorDrag == false) //输入鼠标左键且鼠标状态未改变时
            {
                Cursor.SetCursor(cursorTexture[1], hotSpot, cursorMode);    //改变鼠标指针
                cursorDrag = true;                                          //改变鼠标状态
            }
            else if (!Input.GetMouseButton(0) && cursorDrag == true)    //未输入左键时
            {
                Cursor.SetCursor(cursorTexture[0], hotSpot, cursorMode);    //同上
                cursorDrag = false;
            }
        }
        else if (cursorInLevel)     //鼠标在关卡元素上时
        {
            if (Input.GetMouseButton(0) && cursorDrag == false) //同上
            {
                Cursor.SetCursor(cursorTexture[3], hotSpot, cursorMode);
                cursorDrag = true;
            }
            else if (!Input.GetMouseButton(0) && cursorDrag == true)
            {
                Cursor.SetCursor(cursorTexture[2], hotSpot, cursorMode);
                cursorDrag = false;
            }
        }
    }

    void ScoreManagement()  //当前积分UI管理
    {
        if (gameState != GameState.End)     //如果游戏状态非结束
        {
            if (scoreInText < score)        //UI中积分小于实际积分时
            {
                if (!oncePerformed)         //未执行设定时间条件操作时
                {
                    nextChange = changeRate + Time.time;
                    oncePerformed = true;
                }
                else if (Time.time >= nextChange)   //时间条件满足时（每0.1秒）
                {
                    scoreInText += Random.Range(200, 1000); //每帧不均匀增加UI中积分以达到UI积分自然渐变目的
                    scoreInText -= scoreInText % 10;        //设定积分必须为10的倍数
                    if (scoreInText <= score) scoreText.text = "Score: " + (int)scoreInText;    //UI中积分小于实际积分时将UI设为UI积分
                    oncePerformed = false;
                }
            }
            else if (scoreInText > score)   //UI中积分大于实际积分时
            {
                scoreInText = score;        //UI积分与实际积分同步
                scoreText.text = "Score: " + (int)scoreInText;
            }
        }
        else scoreText.text = "Score: " + score;    //游戏状态为结束时不进行渐变，直接将UI设为实际积分
    }

    void HighScoreManagement()
    {
        
    }

    void LevelResult()  //关卡结果判断
    {
        if (pigTotalNum <= 0)   //胜利判断（优先级大）
        {
            gameState = GameState.End;  //改变游戏状态
            Debug.Log("You Win!");
            
        }
        else if (birdTotalNum <= 0)     //失败判断（优先级小）
        {
            gameState = GameState.End;  //改变游戏状态
            Debug.Log("You Lose!");
            
        }
    }

    public void LevelPause()    //暂停（点击暂停按钮后执行）
    {
        //PauseMenu.GetComponent<RectTransform>().Translate(Vector3.right * 200);
        cursorInLevel = false;      //改变鼠标状态
        GreyPanel.SetActive(true);  //改变灰色画板（窗口）状态
        PauseMenu.SetActive(true);  //改变暂停界面状态

        Time.timeScale = 0; //使游戏暂停
    }
    public void LevelContinue() //取消暂停（点击继续按钮后执行）
    {
        //PauseMenu.GetComponent<RectTransform>().Translate(Vector3.left * 200);
        cursorInLevel = true;        //改变鼠标状态
        GreyPanel.SetActive(false);  //改变灰色画板（窗口）状态
        PauseMenu.SetActive(false);  //改变暂停界面状态

        Time.timeScale = 1; //使游戏取消暂停
    }
    public void ActivateExitWindow()    //激活是否退出游戏的窗口（按钮）
    {
        GreyPanel.SetActive(true);
        ExitWindow.SetActive(true);
    }
    public void RegretToQuit()  //决定不退出游戏后操作（按钮）
    {
        GreyPanel.SetActive(false);
        ExitWindow.SetActive(false);
    }
    public void QuitGame()      //退出游戏（按钮）
    {
        Application.Quit();
        Debug.Log("Game Quitted.");
    }
    
    public static Vector3 GetWorldMousePos()    //获取鼠标--世界坐标
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }
    public static Vector3 GetViewPos(Vector3 viewPos)   //获取相机视觉(0-1)--世界坐标
    {
        return Camera.main.ViewportToWorldPoint(viewPos);
    }
}
