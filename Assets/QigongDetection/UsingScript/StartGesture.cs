using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rfilkov.kinect;
using System.Linq;


namespace com.rfilkov.components
{
    public class StartGesture : MonoBehaviour
    {

        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("UI-Text to display gesture-listener messages and gesture information.")]
        public UnityEngine.UI.Text gestureInfo;

        [Tooltip("The left muse joint we want to track.")]
        public KinectInterop.JointType leftmuse = KinectInterop.JointType.HandtipLeft;

        [Tooltip("The right muse joint we want to track.")]
        public KinectInterop.JointType rightmuse = KinectInterop.JointType.HandtipRight;

        private static ModelGestureListener instance = null;

        //private bool progressDisplayed;

        //private float progressGestureTime;

        private bool pull;

        private Vector3 gesturePosition = Vector3.zero;

        public RectTransform canvasRect;
        public RectTransform cursorRect;

        private Vector2 canvasSize;
        private Vector2 cursorSize;
        private Vector2 cursorOffset;




        public static ModelGestureListener Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsPull()
        {
            return pull;
        }

        public void UserDetected(ulong userId, int userIndex)
        {
            // the gestures are allowed for the selected user only
            KinectGestureManager gestureManager = KinectManager.Instance.gestureManager;
            if (!gestureManager || (userIndex != playerIndex))
                return;

            // set the gestures to detect
            gestureManager.DetectGesture(userId, GestureType.Pull);


            if (gestureInfo != null)
            {
                gestureInfo.text = "pull the botton to choose the setting.";
            }
        }

        public void UserLost(ulong userId, int userIndex)
        {
            // the gestures are allowed for the primary user only
            if (userIndex != playerIndex)
                return;

            if (gestureInfo != null)
            {
                gestureInfo.text = string.Empty;
            }
        }


        void Start()
        {


        }

        // Update is called once per frame
        void Update()
        {
            KinectManager kinectManager = KinectManager.Instance;
            //KinectGestureManager gestureManager = KinectManager.Instance.gestureManager;

            ulong userId = kinectManager.GetUserIdByIndex(playerIndex);

            Dictionary<int, KinectInterop.JointType> selectMuse = new Dictionary<int, KinectInterop.JointType>();
            selectMuse.Add(1, leftmuse);
            selectMuse.Add(2, rightmuse);
            List<Vector3> positions = new List<Vector3>();
            Dictionary<KinectInterop.JointType, Vector3> musePosition = new Dictionary<KinectInterop.JointType, Vector3>();

            foreach (KinectInterop.JointType value in selectMuse.Values)
            {
                gesturePosition = kinectManager.GetJointKinectPosition(userId, value, true);

                Vector3 gp = gesturePosition;
                positions.Add(gp);
                musePosition[value] = gp;
                string debugOutput = string.Join("\n", musePosition.Select(p => string.Format("{0}: ({1:F2}, {2:F2}, {3:F2})", p.Key, p.Value.x, p.Value.y, p.Value.z)));
                if (gestureInfo)
                {
                    gestureInfo.text = debugOutput;
                }                
            }

        }
    }
}
