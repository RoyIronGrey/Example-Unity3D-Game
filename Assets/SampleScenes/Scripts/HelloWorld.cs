using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HelloWorld : MonoBehaviour {

    public int testInt = 1;

    private float m_LastUpdateShowTime = 0f;    //上一次更新帧率的时间;  

  //  private float m_UpdateShowDeltaTime = 0.01f;//更新帧率的时间间隔;  

    private int m_FrameUpdate = 0;//帧数;  

    private double m_FPS = 0;     //帧率

    double cpuRate = 0.0d;
    private string[] args;

    StreamWriter sw;
    private bool swIsOk = false; 

    private bool IsTiming;  //是否开始计时
    private float CountDown; //倒计时

    private AndroidJavaObject activity;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        CreateFile(Application.persistentDataPath, "Log.txt");
        args = new string[3];
    }

    // Use this for initialization
    void Start () {
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
        //Debug.Log("Hello World");
        //Debug.Log(Time.realtimeSinceStartup);

        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        //activity.Call("LogUtil",Application.persistentDataPath);
        //CreateFile(Application.persistentDataPath, "Log.txt");
        //args = new string[3];
        string arg = "-------------Hello World--------------";
        activity.Call("LogUtil", arg);

    }
	
	// Update is called once per frame
	void Update () {
        ExitDetection(); //调用 退出检测函数
        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= 0.5 && swIsOk)
        {
            m_FPS = Math.Round((m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime)),0);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
            cpuRate = Math.Round(activity.Call<double>("getAvgCpu"),2);

            args[0] = DateTime.Now.ToString("HH:mm:ss.fff");
            args[1] = cpuRate.ToString()+'%';
            args[2] = m_FPS.ToString();

            WriteFile(args);
        }
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 50;

        //GUI.Label(new Rect(10, 10, Screen.width, Screen.height), "Hello World");
        //System.Threading.Thread.Sleep(1000);
        GUI.Label(new Rect(100, 100, 1000, 200), "FPS: " + m_FPS);
        GUI.Label(new Rect(100, 200, 2000, 200),"CPU: "+cpuRate);
    }

    void CreateFile(string path, string name)
    {
        FileInfo file = new FileInfo(path + "//" + name);
        if (!file.Exists)
        {
            file.Create(); 
        }
  
        sw = new StreamWriter(path + "//" + name, true);
        swIsOk = true;
    }

    void WriteFile(string []args)
    {
        for(int i = 0; i < args.Length; i++)
        {
            sw.Write(args[i]+'\t');
        }
        sw.Write("\r\n");
    }

    void CloseFile()
    {
        sw.Close();
        sw.Dispose();
    }

    ArrayList LoadFile(string path, string name)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }catch(Exception e)
        {
            CreateFile(path, name);
            return null;
        }
        string line;
        ArrayList arrayList = new ArrayList();
        while((line = sr.ReadLine()) != null)
        {
            arrayList.Add(line);
        }

        sr.Close();
        sr.Dispose();

        return arrayList;
    }

    void DeleteFile(string path, string name)
    {
        File.Delete(path + "//" + name);
    }

    private void ExitDetection()
    {
        if (Input.GetKeyDown(KeyCode.Escape))            //如果按下退出键
        {
            if (CountDown == 0)                          //当倒计时时间等于0的时候
            {
                CountDown = Time.time;                   //把游戏开始时间，赋值给 CountDown
                IsTiming = true;                        //开始计时
            }
            else
            {
                CloseFile();
                Application.Quit();                      //退出游戏
            }
        }

        if (IsTiming) //如果 IsTiming 为 true 
        {
            if ((Time.time - CountDown) > 2.0)           //如果 两次点击时间间隔大于2秒
            {
                CountDown = 0;                           //倒计时时间归零
                IsTiming = false;                       //关闭倒计时
            }
        }
    }
}
