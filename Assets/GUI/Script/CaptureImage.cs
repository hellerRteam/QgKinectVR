using System.IO;
using UnityEngine;

public class CaptureImage : MonoBehaviour
{
    // 设置要保存的图像的分辨率
    public int width = 1920;
    public int height = 1080;

    // 设置保存图像的路径和文件名
    public string savePath = " Assets\\GUI\\picture\\9641715314367_ 1.png";

    // 在需要保存图像的地方调用此函数
    public void SaveImage()
    {
        // 设置摄像机的目标纹理为一个新的RenderTexture
        RenderTexture rt = new RenderTexture(width, height, 24);
        Camera.main.targetTexture = rt;

        // 渲染摄像机到目标纹理
        Camera.main.Render();

        // 激活目标纹理并将其读取到一张新的2D纹理中
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // 将2D纹理转换为字节数组并保存为图像文件
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

        // 释放目标纹理
        RenderTexture.active = null;
        Camera.main.targetTexture = null;
        rt.Release();
    }
}

