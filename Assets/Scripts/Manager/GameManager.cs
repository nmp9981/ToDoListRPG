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
    #region µ•¿Ã≈Õ
    private int _playerLv;
    private int _playerHP;
    private int _playerExp;

    public int PlayerLV { get { return _playerLv; } set { _playerLv = value; } }
    public int PlayerHP { get { return _playerHP; } set { _playerHP = value; } }
    public int PlayerExp {  get { return _playerExp; } set {  _playerExp = value; } }
    #endregion
}
