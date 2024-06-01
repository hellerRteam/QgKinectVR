using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;

public class informationRecorder : MonoBehaviour
{
    [SerializeField] public Model model;
    [SerializeField] public string careTaker;
    [SerializeField] public string trainDate;
    [SerializeField] public string fileNameTest;
    [SerializeField] public string fileNameFormal;
    [SerializeField] public string filePath = Application.streamingAssetsPath;
    public Data Trainingdata;
    private string filename;
    public enum Model : int { test = 0 , formal = 1};
    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.streamingAssetsPath;
        if (model == Model.test)
        {
            filename = filePath + "/" + fileNameTest + ".csv";
        }
        else if (model == Model.formal)
        {
            filename = filePath + "/" + fileNameFormal + ".csv";
        }
        Trainingdata = GameObject.Find("Data").GetComponent<Data>();
        DontDestroyOnLoad(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnApplicationQuit()
    {
        print(6);
        DataTable dt = new DataTable("Sheet1");
        dt.Columns.Add("ѵ������");
        dt.Columns.Add("�㻤��");
        dt.Columns.Add("����");
        dt.Columns.Add("���");
        dt.Columns.Add("�Ա�");
        dt.Columns.Add("����");

        dt.Columns.Add("����1��ϰʱ��");
        dt.Columns.Add("����2��ϰʱ��");
        dt.Columns.Add("����3��ϰʱ��");
        dt.Columns.Add("����4��ϰʱ��");
        dt.Columns.Add("����5��ϰʱ��");
        dt.Columns.Add("����6��ϰʱ��");
        dt.Columns.Add("����7��ϰʱ��");
        dt.Columns.Add("����8��ϰʱ��");

        dt.Columns.Add("����1׼ȷ��");
        dt.Columns.Add("����2׼ȷ��");
        dt.Columns.Add("����3׼ȷ��");
        dt.Columns.Add("����4׼ȷ��");
        dt.Columns.Add("����5׼ȷ��");
        dt.Columns.Add("����6׼ȷ��");
        dt.Columns.Add("����7׼ȷ��");
        dt.Columns.Add("����8׼ȷ��");
        DataRow dr = dt.NewRow();
        dr[0] = trainDate;
        dr[1] = careTaker;
        dr[2] = UserData.data[0];
        dr[3] = UserData.data[1];
        dr[4] = UserData.data[2];
        dr[5] = UserData.data[3];

        for (int i = 0; i < Trainingdata.getTotalAction(); i++) {
            dr[6+i] = Trainingdata.ActionTrainingTime[i];
            dr[6 + Trainingdata.getTotalAction() + i] = Trainingdata.ActionAccuracySum[i] / Trainingdata.ActionAccuracyCnt[i];
        }

        dt.Rows.Add(dr);
        
        SaveFile(filename, dt);
    }

    void SaveFile(string filePath, DataTable data)
    {
        FileInfo file = new FileInfo(filePath);
        print("creat");
        if (!file.Exists)
        {
            print("creat2");
            file.Directory.Create();
        }
        FileMode mode = File.Exists(filePath) ? FileMode.Append : FileMode.Create;
        using (FileStream fs = new FileStream(filePath, mode, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                string dataTable = "";
                if (mode == FileMode.Create)
                {
                    print("creat3");

                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        dataTable += data.Columns[i].ColumnName.ToString();
                        if (i < data.Columns.Count - 1)
                        {
                            dataTable += ",";
                        }
                    }
                    sw.WriteLine(dataTable);
                }
                //д��ÿһ��ÿһ�е�����
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    dataTable = "";
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        string str = data.Rows[i][j].ToString();
                        dataTable += str;
                        if (j < data.Columns.Count - 1)
                        {
                            dataTable += ",";
                        }
                    }
                    sw.WriteLine(dataTable);
                }
                sw.Close();
                fs.Close();
            }
        }
    }
}
