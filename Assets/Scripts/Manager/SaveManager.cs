using System.IO;
using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData Data { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
    private string BackupPath => Path.Combine(Application.persistentDataPath, "save.backup.json");
    private string TempPath => Path.Combine(Application.persistentDataPath, "save.tmp.json");

    private const float AUTO_SAVE_INTERVAL = 30f;
    private float autoSaveTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    private void Update()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= AUTO_SAVE_INTERVAL)
        {
            autoSaveTimer = 0f;
            Save();
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                Data = JsonUtility.FromJson<SaveData>(json);

                if (Data != null)
                {
                    Debug.Log($"[SaveManager] 로드 완료: Lv.{Data.level} {Data.playerName}");
                    return;
                }
            }

            // 백업 시도
            if (File.Exists(BackupPath))
            {
                Debug.LogWarning("[SaveManager] 메인 파일 손상, 백업에서 복구");
                string json = File.ReadAllText(BackupPath);
                Data = JsonUtility.FromJson<SaveData>(json);

                if (Data != null)
                {
                    Save();
                    return;
                }
            }

            // 새로 생성
            Debug.Log("[SaveManager] 저장 파일 없음, 새로 생성");
            Data = new SaveData();
            Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 로드 실패: {e.Message}");
            Data = new SaveData();
        }
    }

    public void Save()
    {
        if (Data == null) return;

        try
        {
            Data.lastSavedAt = DateTime.Now.ToString("o");

            string json = JsonUtility.ToJson(Data, true);

            // 안전 저장 (임시 파일 → 교체 + 백업)
            File.WriteAllText(TempPath, json);

            if (File.Exists(SavePath))
                File.Copy(SavePath, BackupPath, true);

            File.Move(TempPath, SavePath);

            Debug.Log("[SaveManager] 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 저장 실패: {e.Message}");
        }
    }

    private void OnApplicationQuit() => Save();
    private void OnApplicationPause(bool pause) { if (pause) Save(); }
}
