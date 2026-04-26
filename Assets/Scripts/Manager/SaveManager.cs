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
        // 싱글톤
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

    /// <summary>
    /// 저장 파일 로드. 없으면 새로 생성.
    /// </summary>
    public void Load()
    {
        try
        {
            // 1. 메인 파일 시도
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                Data = JsonUtility.FromJson<SaveData>(json);

                if (Data != null)
                {
                    Debug.Log($"[SaveManager] 로드 완료: {SavePath}");
                    CheckDateRollover();  // 자정 처리
                    return;
                }
            }

            // 2. 메인 실패 시 백업 시도
            if (File.Exists(BackupPath))
            {
                Debug.LogWarning("[SaveManager] 메인 파일 손상, 백업에서 복구");
                string json = File.ReadAllText(BackupPath);
                Data = JsonUtility.FromJson<SaveData>(json);

                if (Data != null)
                {
                    Save();  // 복구 즉시 메인에 다시 저장
                    CheckDateRollover();
                    return;
                }
            }

            // 3. 새 데이터 생성
            Debug.Log("[SaveManager] 저장 파일 없음, 새로 생성");
            Data = new SaveData();
            Data.lastActiveDate = DateTime.Now.ToString("yyyy-MM-dd");
            Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 로드 실패: {e.Message}");
            Data = new SaveData();
        }
    }

    /// <summary>
    /// 저장 (안전 저장 패턴)
    /// </summary>
    public void Save()
    {
        if (Data == null) return;

        try
        {
            Data.lastSavedAt = DateTime.Now.ToString("o");
            string json = JsonUtility.ToJson(Data, true);

            File.WriteAllText(TempPath, json);

            if (File.Exists(SavePath))
                File.Copy(SavePath, BackupPath, true);

            if (File.Exists(SavePath))
                File.Delete(SavePath);
            File.Move(TempPath, SavePath);

            Debug.Log("[SaveManager] 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 저장 실패: {e.Message}");

            if (File.Exists(TempPath))
            {
                try { File.Delete(TempPath); } catch { }
            }
        }
    }

    /// <summary>
    /// 자정 전환 처리: 어제까지의 오늘 통계를 dailyRecords에 저장하고 리셋
    /// </summary>
    public void CheckDateRollover()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (string.IsNullOrEmpty(Data.lastActiveDate))
        {
            Data.lastActiveDate = today;
            return;
        }

        if (Data.lastActiveDate == today) return;

        // 날짜 바뀜 → 어제 기록을 dailyRecords에 저장
        var record = new DailyFocusRecord
        {
            date = Data.lastActiveDate,
            focusSeconds = Data.todayConcentrateSeconds  // PlayerData 안에 있다면 경로 조정
        };

        // 같은 날짜 레코드 있으면 업데이트, 없으면 추가
        var existing = Data.dailyRecords.Find(r => r.date == Data.lastActiveDate);
        if (existing != null)
            existing.focusSeconds = record.focusSeconds;
        else
            Data.dailyRecords.Add(record);

        // 오늘 통계 리셋
        Data.todayConcentrateSeconds = 0;
        Data.countOtherAction = 0;
        Data.todayMissionCompleted = 0;
        Data.todayLossHP = 0f;

        Data.lastActiveDate = today;

        Debug.Log($"[SaveManager] 날짜 전환: {today} 시작");
    }

    private void OnApplicationQuit() => Save();
    private void OnApplicationPause(bool pause) { if (pause) Save(); }

    [ContextMenu("저장 폴더 열기")]
    public void OpenSaveFolder()
    {
        Application.OpenURL(Application.persistentDataPath);
    }
}
