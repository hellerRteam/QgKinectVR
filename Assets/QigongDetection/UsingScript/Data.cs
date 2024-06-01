using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public string userID = "test";
    public const int TotalAction = 8;
    public float[] ActionTrainingTime = new float[TotalAction];
    public float[] ActionAccuracySum = new float[TotalAction];
    public int[] ActionAccuracyCnt = new int[TotalAction];
    public float TargetAccuracy;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < TotalAction; i++) {
            ActionTrainingTime[i] = 0;
            ActionAccuracySum[i] = 0;
            ActionAccuracyCnt[i] = 0;
        }
        TargetAccuracy = 0;
        DontDestroyOnLoad(this);
    }

    public void AccumulateTrainingTime(int idx,float time) {
        ActionTrainingTime[idx] += time;
    }
    public void AccumulateAccuracy(int idx, float accuracy)
    {
        ActionAccuracySum[idx] += accuracy;
        ActionAccuracyCnt[idx] += 1;
    }

    public int getTotalAction() { return TotalAction; }
    public float getAverageAccuracy() {
        float averageAccuracy = 0;
        int totalCnt = 0;
        for (int i = 0; i < TotalAction; i++)
        {
            averageAccuracy+=ActionAccuracySum[i];
            totalCnt+=ActionAccuracyCnt[i];
        }
        return averageAccuracy/ totalCnt;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
