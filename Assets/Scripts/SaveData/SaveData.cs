using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // 캐릭터 기본 정보
    public string playerName = "";
    public int level = 1;
    public int exp = 0;
    public float hp = 100f;

    // 칭호 (통합 점수 방식)
    public int titleIdx = 0;  // 0=브론즈5, 4=브론즈1, 5=실버5...

    //미션 기록
    public List<MissionInfo> activeMissions = new List<MissionInfo>();

    // 누적 기록 (통계와 별개의 "평생 누적")
    public long totalFocusSeconds = 0;
    public int totalMissionsCompleted = 0;//완료 미션 개수

    // 저장 파일 버전 (나중에 구조 바뀔 때를 위해)
    public int saveVersion = 1;

    // 마지막 저장 시각 (시간 조작 감지용, 선택)
    public string lastSavedAt = "";
}
