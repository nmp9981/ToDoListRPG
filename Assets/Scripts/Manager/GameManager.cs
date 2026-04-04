using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        _player = new PlayerInfo();
        _player.InitPlayerInfo();
    }
    /// <summary>
    /// 요구 경험치 계산
    /// </summary>
    /// <returns></returns>
    public int CalRequireExp(int lv)
    {
        if (lv == 1) return 300;

        if (lv < 11) return lv * 200;

        return 2000*(int)Mathf.Pow(RequireExpRate, lv-10);
    }
    #region 데이터
    public PlayerInfo _player;
    public GameObject _missionPrefab;
    public List<SymbolInfo> _playerLvSymbolImage = new();
    public List<MissionInfo> _failMissionList = new();

    private int _requireExpRate = 105;//경험치 배율
    private int _symbolMaxCount = 20;//칭호 개수
    public int RequireExpRate { get { return _requireExpRate; } }
    public int SymbolMaxCount { get { return _symbolMaxCount; } }
    #endregion
}
