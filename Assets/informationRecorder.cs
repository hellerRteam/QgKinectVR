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
        dt.Columns.Add("训练日期");
        dt.Columns.Add("陪护者");
        dt.Columns.Add("姓名");
        dt.Columns.Add("编号");
        dt.Columns.Add("性别");
        dt.Columns.Add("年龄");

        dt.Columns.Add("动作1练习时间");
        dt.Columns.Add("动作2练习时间");
        dt.Columns.Add("动作3练习时间");
        dt.Columns.Add("动作4练习时间");
        dt.Columns.Add("动作5练习时间");
        dt.Columns.Add("动作6练习时间");
        dt.Columns.Add("动作7练习时间");
        dt.Columns.Add("动作8练习时间");

        dt.Columns.Add("动作1准确度");
        dt.Columns.Add("动作2准确度");
        dt.Columns.Add("动作3准确度");
        dt.Columns.Add("动作4准确度");
        dt.Columns.Add("动作5准确度");
        dt.Columns.Add("动作6准确度");
        dt.Columns.Add("动作7准确度");
        dt.Columns.Add("动作8准确度");
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
                //写入每一行每一列的数据
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
