using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // ===== 캐릭터 기본 정보 =====
    public string playerName = "";
    public int level = 1;
    public int exp = 0;
    public float hp = 100f;
    public int titleIdx = 0;  // 0=브론즈5, 4=브론즈1, 5=실버5...

    // ===== 미션 =====
    public List<MissionData> activeMissions = new List<MissionData>();

    // ===== 일별 집중 기록 =====
    public List<DailyFocusRecord> dailyRecords = new List<DailyFocusRecord>();

    // ==== 허용 프로그램 리스트
    public Whitelistdata whitelistdata = new Whitelistdata();

    // ===== 평생 누적 통계 =====
    public long totalFocusSeconds = 0;
    public int totalMissionsCompleted = 0;

    // ===== 오늘의 통계 (자정에 리셋) =====
    public int todayConcentrateSeconds = 0;
    public int countOtherAction = 0;
    public int todayMissionCompleted = 0;
    public float todayLossHP = 0f;

    // ===== 메타 =====
    public string lastActiveDate = "";  // 자정 전환 감지용
    public int saveVersion = 1;
    public string lastSavedAt = "";
}

[Serializable]
public class DailyFocusRecord
{
    public string date;        // "2026-04-25"
    public float focusSeconds;
}