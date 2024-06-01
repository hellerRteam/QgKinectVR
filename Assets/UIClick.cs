using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.rfilkov.kinect;

//��������Ƿ��ƶ�����ťλ�ò��������ֶ���
//rightHandImage��ģ����UI
//btnImage����ťUI

public class UIClick : MonoBehaviour
{
    public Camera MainCamera;
    public Canvas canvas;
    public Image rightHandImage;
    public Image btnImage1;
    public Vector2 rightHandPosition;


    KinectManager _manager;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_manager == null)
        {
            _manager = KinectManager.Instance;
            print(1);        
        }

        if (_manager && _manager.IsInitialized())
        {
            print(2);
            if (_manager.IsUserDetected())
            {
                print(3);
                long userId = (long)_manager.GetPrimaryUserID();
                int jointIndex = (int)KinectInterop.JointType.HandRight;
                if (_manager.IsJointTracked((ulong)userId, jointIndex))
                {
                    print(4);
                    Rect backgroundRect = MainCamera.pixelRect;
                    Vector3 rightHandPos = _manager.GetJointPosColorOverlay(userId, jointIndex, MainCamera, backgroundRect);

                    Vector3 rightHandScreenPos = Camera.main.WorldToScreenPoint(rightHandPos);
                    Vector2 rightHandScreenPosTemp = new Vector2(rightHandScreenPos.x, rightHandScreenPos.y);

                    Vector2 rightHandUguiPos;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, rightHandScreenPosTemp, null, out rightHandUguiPos))
                    {
                        RectTransform rightHandRTF = rightHandImage.transform as RectTransform;
                        rightHandRTF.anchoredPosition = rightHandUguiPos;
                    }

                    if (RectTransformUtility.RectangleContainsScreenPoint(btnImage1.rectTransform, rightHandScreenPosTemp, null))
                    {
                        Debug.Log("�ڰ�ť��");
                        KinectInterop.HandState rightHandState = _manager.GetRightHandState((ulong)userId);
                        if (rightHandState == KinectInterop.HandState.Closed)
                        {
                            Debug.Log("������ȭ");
                        }
                    }
                    else
                    {
                        Debug.Log("�ڰ�ť��");
                    }
                }
            }
        }
    }
}
