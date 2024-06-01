using System.IO;
using UnityEngine;

public class CaptureImage : MonoBehaviour
{
    // ����Ҫ�����ͼ��ķֱ���
    public int width = 1920;
    public int height = 1080;

    // ���ñ���ͼ���·�����ļ���
    public string savePath = " Assets\\GUI\\picture\\9641715314367_ 1.png";

    // ����Ҫ����ͼ��ĵط����ô˺���
    public void SaveImage()
    {
        // �����������Ŀ������Ϊһ���µ�RenderTexture
        RenderTexture rt = new RenderTexture(width, height, 24);
        Camera.main.targetTexture = rt;

        // ��Ⱦ�������Ŀ������
        Camera.main.Render();

        // ����Ŀ�����������ȡ��һ���µ�2D������
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // ��2D����ת��Ϊ�ֽ����鲢����Ϊͼ���ļ�
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

        // �ͷ�Ŀ������
        RenderTexture.active = null;
        Camera.main.targetTexture = null;
        rt.Release();
    }
}

