using System;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이 모드
/// </summary>
public enum PlayMode
{
    General,
    Concentration,
    Count
}

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance { get { Init(); return _instance; } }
    
    static void Init()
    {
        if (_instance == null)
        {
            GameObject gm = GameObject.Find("GameManager");
            if (gm == null)
            {
                gm = new GameObject { name = "GameManager" };

                gm.AddComponent<GameManager>();
            }
            DontDestroyOnLoad(gm);
            _instance = gm.GetComponent<GameManager>();
        }
    }

    private void Awake()
    {
        //FindPlayer();
    }

    /// <summary>
    /// 플레이어 찾기
    /// </summary>
    public void FindPlayer()
    {
        // 새 씬에서 PlayerInfo 다시 찾기
        if(_player == null)
        {
            _player = FindAnyObjectByType<PlayerInfo>();
        }
        Debug.Log($"[GameManager] 씬 '{SceneManager.GetActiveScene().name}' 로드됨, _player = {(_player == null ? "null" : "OK")}");
    }


    /// <summary>
    /// 요구 경험치 계산
    /// </summary>
    /// <returns></returns>
    public int CalRequireExp(int lv)
    {
        if (lv == 1) return 300;

        if (lv < 11) return lv * 200;

        float rate = (float)_requireExpRate / 100f;
        return 2000*(int)Mathf.Pow(rate, lv-10);
    }

    #region 데이터
    public PlayerInfo _player;
    public GameObject _missionPrefab;
    public List<SymbolInfo> _playerLvSymbolImage = new();
    public List<MissionInfo> _failMissionList = new();

    [SerializeField] private PlayMode _playMode = PlayMode.General;//플레이 모드
    private int _requireExpRate = 105;//경험치 배율
    private int _symbolMaxCount = 20;//칭호 개수
    [SerializeField] private float _concentrateContinueTime;//집중 지속 시간
    public PlayMode PlayMode { get { return _playMode; } set { _playMode = value; } }
    public int RequireExpRate { get { return _requireExpRate; } }
    public int SymbolMaxCount { get { return _symbolMaxCount; } }
    public float ConcentrateContinueTime { get { return _concentrateContinueTime; }set { _concentrateContinueTime = value; } }
    #endregion
}
