using UnityEngine;
using System.Collections;
using com.rfilkov.kinect;
using System.Collections.Generic;
using System.Linq;


namespace com.rfilkov.components
{
    public class JointViewer : MonoBehaviour
    {
        [Tooltip("Depth sensor index - 0 is the 1st one, 1 - the 2nd one, etc. -1 means the sensor doesn't matter")]
        public int sensorIndex = -1;

        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("The Kinect joints we want to track.")]
        public int initialJoint = 0;
        public int finalJoint = 31;
        //public KinectInterop.JointType trackedJoint = KinectInterop.JointType.Pelvis;

        [Tooltip("Whether the movement is relative to transform's initial position, or is in absolute coordinates.")]
        public bool relToInitialPos = false;

        [Tooltip("Whether the z-movement is inverted or not.")]
        public bool invertedZMovement = false;

        [Tooltip("Scene object that will be used to represent the sensor's position and rotation in the scene.")]
        public Transform sensorTransform;

        //public bool moveTransform = true;

        [Tooltip("Smooth factor used for the joint position smoothing.")]
        public float smoothFactor = 0f;

        [Tooltip("UI-Text to display the current joint position.")]
        public UnityEngine.UI.Text debugText;


        private Vector3 initialPosition = Vector3.zero;
        private ulong currentUserId = 0;
        private Vector3 initialUserOffset = Vector3.zero;

        private Vector3 vPosJoint = Vector3.zero;


        void Start()
        {
            initialPosition = transform.position;

        }

        void Update()
        {
            KinectManager kinectManager = KinectManager.Instance;
            Dictionary<int, KinectInterop.JointType> jointDictionary = new Dictionary<int, KinectInterop.JointType>();
            jointDictionary.Add(0, KinectInterop.JointType.Pelvis);
            jointDictionary.Add(1, KinectInterop.JointType.SpineNaval);
            jointDictionary.Add(2, KinectInterop.JointType.SpineChest);
            jointDictionary.Add(3, KinectInterop.JointType.Neck);
            jointDictionary.Add(4, KinectInterop.JointType.Head);
            jointDictionary.Add(5, KinectInterop.JointType.ClavicleLeft);
            jointDictionary.Add(6, KinectInterop.JointType.ShoulderLeft);
            jointDictionary.Add(7, KinectInterop.JointType.ElbowLeft);
            jointDictionary.Add(8, KinectInterop.JointType.WristLeft);
            jointDictionary.Add(9, KinectInterop.JointType.HandLeft);
            jointDictionary.Add(10, KinectInterop.JointType.ClavicleRight);
            jointDictionary.Add(11, KinectInterop.JointType.ShoulderRight);
            jointDictionary.Add(12, KinectInterop.JointType.ElbowRight);
            jointDictionary.Add(13, KinectInterop.JointType.WristRight);
            jointDictionary.Add(14, KinectInterop.JointType.HandRight);
            jointDictionary.Add(15, KinectInterop.JointType.HipLeft);
            jointDictionary.Add(16, KinectInterop.JointType.KneeLeft);
            jointDictionary.Add(17, KinectInterop.JointType.AnkleLeft);
            jointDictionary.Add(18, KinectInterop.JointType.FootLeft);
            jointDictionary.Add(19, KinectInterop.JointType.HipRight);
            jointDictionary.Add(20, KinectInterop.JointType.KneeRight);
            jointDictionary.Add(21, KinectInterop.JointType.AnkleRight);
            jointDictionary.Add(22, KinectInterop.JointType.FootRight);
            jointDictionary.Add(23, KinectInterop.JointType.Nose);
            jointDictionary.Add(24, KinectInterop.JointType.EyeLeft);
            jointDictionary.Add(25, KinectInterop.JointType.EarLeft);
            jointDictionary.Add(26, KinectInterop.JointType.EyeRight);
            jointDictionary.Add(27, KinectInterop.JointType.EarRight);
            jointDictionary.Add(28, KinectInterop.JointType.HandtipLeft);
            jointDictionary.Add(29, KinectInterop.JointType.ThumbLeft);
            jointDictionary.Add(30, KinectInterop.JointType.HandtipRight);
            jointDictionary.Add(31, KinectInterop.JointType.ThumbRight);

            //int numElements = finalJoint - initialJoint + 1;

            //int[] selectJoints = new int[numElements];
            //for (int i = 0; i < numElements; i++)
            //{
            //    selectJoints[i] = initialJoint + i;
            //}

            Dictionary<int, KinectInterop.JointType> selectJoints = jointDictionary.Where(kv => kv.Key >= initialJoint && kv.Key <= finalJoint).ToDictionary(kv => kv.Key, kv => kv.Value);



            if (kinectManager && kinectManager.IsInitialized())
                {
                if (sensorIndex >= 0 || kinectManager.IsUserDetected(playerIndex))
                {
                    ulong userId = sensorIndex < 0 ? kinectManager.GetUserIdByIndex(playerIndex) : (ulong)playerIndex;
                    List<Vector3> positions = new List<Vector3>();
                    Dictionary<KinectInterop.JointType, Vector3> jointPositions = new Dictionary<KinectInterop.JointType, Vector3>();
                    foreach (KinectInterop.JointType value in selectJoints.Values)
                    {
                        if (sensorIndex >= 0 || kinectManager.IsJointTracked(userId, value))
                        {
                            if (sensorTransform != null)
                            {
                                if (sensorIndex < 0)
                                    vPosJoint = kinectManager.GetJointKinectPosition(userId, value, true);
                                else
                                    vPosJoint = kinectManager.GetSensorJointKinectPosition(sensorIndex, (int)userId, value, true);
                            }
                            else
                            {
                                if (sensorIndex < 0)
                                    vPosJoint = kinectManager.GetJointPosition(userId, value);
                                else
                                    vPosJoint = kinectManager.GetSensorJointPosition(sensorIndex, (int)userId, value);
                            }

                            vPosJoint.z = invertedZMovement ? -vPosJoint.z : vPosJoint.z;
                        }

                        if (sensorTransform)
                        {
                            vPosJoint = sensorTransform.TransformPoint(vPosJoint);
                        }

                        if (userId != currentUserId)
                        {
                            currentUserId = userId;
                            initialUserOffset = vPosJoint;
                        }

                        Vector3 vPosObject = relToInitialPos ? initialPosition + (vPosJoint - initialUserOffset) : vPosJoint;
                        positions.Add(vPosObject);
                        jointPositions[value] = vPosObject;

                        string debugOutput = string.Join("\n", jointPositions.Select(p => string.Format("{0}: ({1:F2}, {2:F2}, {3:F2})", p.Key, p.Value.x, p.Value.y, p.Value.z)));
                        if (debugText)
                        {
                             debugText.text = debugOutput;
                            //debugText.text = string.Format("{0} - ({1:F2}, {2:F2}, {3:F2})", value,
                            //                                                       vPosObject.x, vPosObject.y, vPosObject.z);
                        }
                        if (smoothFactor != 0f)
                            transform.position = Vector3.Lerp(transform.position, vPosObject, smoothFactor * Time.deltaTime);
                        else
                            transform.position = vPosObject;
                    }


                }



            }
        }

    }
}
