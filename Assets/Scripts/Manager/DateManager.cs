using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DateManager : MonoBehaviour
{
    static DateManager _dateInstance;
    public static DateManager DateInstance { get { Init(); return _dateInstance; } }

    DateTime _prevDateTime;
    string mainSceneName = "Main";

    private bool _isMidNight = false;
    public bool IsMidNight { get { return _isMidNight; }set { _isMidNight = value; } }

    static void Init()
    {
        if (_dateInstance == null)
        {
            GameObject gm = GameObject.Find("DateManager");
            if (gm == null)
            {
                gm = new GameObject { name = "DateManager" };

                gm.AddComponent<DateManager>();
            }
            DontDestroyOnLoad(gm);
            _dateInstance = gm.GetComponent<DateManager>();
        }
    }

    private void Awake()
    {
        if (_dateInstance != null && _dateInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 시작할 때 현재 시간 저장
        _prevDateTime = DateTime.Now;
        InvokeRepeating("CheckMidnight", 1, 1);
    }

    /// <summary>
    /// 자정 지났는가?
    /// </summary>
    public void CheckMidnight()
    {
        var date = DateTime.Now;
        if (date.Day != _prevDateTime.Day)
        {
            Scene scene = SceneManager.GetActiveScene();
            var player = GameManager.Instance._player;

            if(scene.name == mainSceneName)
            {
                _prevDateTime = date;
                player.InitConcentrateInfo();
            }
            player._weekConcentrateTimeStack.Push(player.ConsumeConcentrateTime);

            DailyFocusRecord dailyFocusRecord = new DailyFocusRecord();
            dailyFocusRecord.date = $"{date.Month}/{date.Day}";
            dailyFocusRecord.focusSeconds = player.ConsumeConcentrateTime;
            player._dailyFocusRecordList.Add(dailyFocusRecord);

            _isMidNight = true;
        }
        else _isMidNight = false;
    }
}
