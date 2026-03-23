using UnityEngine;

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
    #region 데이터
    public PlayerInfo _player;
    public GameObject _missionPrefab;

    private int requireExpRate = 105;//경험치 배율
    public int RequireExpRate { get { return requireExpRate; } }
    #endregion
}
