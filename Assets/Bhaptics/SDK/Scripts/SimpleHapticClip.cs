using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{

    [CreateAssetMenu(fileName = "HapticClip", menuName = "Bhaptics/Create Simple HapticClip")]
    public class SimpleHapticClip : HapticClip
    {
        private static readonly Point[] DefaultPoints =
        {
            new Point(0.5f, 0.5f, 100)
        };


        [SerializeField] private Pos Position = Pos.VestFront;

        [SerializeField] private FeedbackType Mode = FeedbackType.DotMode;

        [SerializeField] private int[] DotPoints = new int[20];

        [SerializeField] private Point[] Points = {new Point(0.5f, 0.5f, 100)};

        [Range(20, 10000)] public int TimeMillis = 1000;

        public override void ResetValues()
        {
            base.ResetValues();

            DotPoints = new int[20];
            Points = DefaultPoints;
        }

        public override void Play(float intensity, float duration, float vestRotationAngleX, float vestRotationOffsetY)
        {
            if (duration <= 0)
                duration = TimeMillis/1000f;

            if (!BhapticsManager.Init)
            {
                BhapticsManager.Initialize();
                return;
            }

            var haptic = BhapticsManager.GetHaptic();

            if (Mode == FeedbackType.DotMode)
            {
                haptic.Submit(keyId, ToPositionType(Position), Convert(DotPoints, intensity), (int)(duration * 1000));
            }
            else
            {
                haptic.Submit(keyId, ToPositionType(Position), Convert(Points, intensity), (int)(duration * 1000));
            }
        }

        public int GetNumberOfPoints()
        {
            return (Mode == FeedbackType.DotMode) ? DotPoints.Length : Points.Length;
        }

        private static List<DotPoint> Convert(int[] points, float intensity)
        {
            var result = new List<DotPoint>();

            for (var i = 0; i < points.Length; i++)
            {
                var p = points[i];
                if (p > 0)
                {
                    result.Add(new DotPoint(i, (int) (p * intensity)));
                }
            }

            return result;
        }

        private static List<PathPoint> Convert(Point[] points, float intensity)
        {
            var result = new List<PathPoint>();

            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                result.Add(new PathPoint(point.X, point.Y, (int)(point.Intensity * intensity)));
            }

            return result;
        }

        static PositionType ToPositionType(Pos pos)
        {
            switch (pos)
            {
                case Pos.Head:
                    return PositionType.Head;
                case Pos.VestFront:
                    return PositionType.VestFront;
                case Pos.VestBack:
                    return PositionType.VestBack;
                case Pos.LeftHand:
                    return PositionType.HandL;
                case Pos.RightHand:
                    return PositionType.HandR;
                case Pos.LeftFoot:
                    return PositionType.FootL;
                case Pos.RightFoot:
                    return PositionType.FootR;
                case Pos.RightForearm:
                    return PositionType.ForearmR;
                case Pos.LeftForearm:
                    return PositionType.ForearmL;
            }

            return PositionType.ForearmR;
        }
    }

    [Serializable]
    public class Point
    {
        [Range(0, 1f)] public float X;
        [Range(0, 1f)] public float Y;

        [Range(0, 100)] public int Intensity;

        public Point(float x, float y, int intensity)
        {
            X = x;
            Y = y;
            Intensity = intensity;
        }
    }

    [Serializable]
    public enum FeedbackType
    {
        DotMode = 1,
        PathMode = 2
    }

    [Serializable]
    public enum Pos
    {
        VestFront,
        VestBack,
        Head,
        RightForearm,
        LeftForearm,
        LeftHand,
        RightHand,
        LeftFoot,
        RightFoot
    }
}