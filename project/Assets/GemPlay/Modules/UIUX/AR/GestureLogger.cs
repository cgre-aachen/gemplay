using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace GemPlay.Modules.UIUX.AR
{
    public static class GestureLogger
    {
        public static void RegisterGestureLoggingEvents(ARGestureInteractor gestureInteractor)
        {
            gestureInteractor.TapGestureRecognizer.onGestureStarted += LogTapGesture;

            gestureInteractor.DragGestureRecognizer.onGestureStarted += LogDragGesture;

            gestureInteractor.TwoFingerDragGestureRecognizer.onGestureStarted += LogTwoFingerDragGesture;

            gestureInteractor.PinchGestureRecognizer.onGestureStarted += LogPinchGesture;

            gestureInteractor.TwistGestureRecognizer.onGestureStarted += LogTwistGesture;
        }

        private static void LogTapGesture(Gesture<TapGesture> gesture)
        {
            string gestureName = "Tap";

            Debug.Log(gestureName + " gesture started.");

            gesture.onFinished += (s) =>
            {
                Debug.Log(gestureName + " gesture finished.");
            };
        }

        private static void LogDragGesture(Gesture<DragGesture> gesture)
        {
            string gestureName = "Drag";

            Debug.Log(gestureName + " gesture started.");

            gesture.onFinished += (s) =>
            {
                Debug.Log(gestureName + " gesture finished.");
            };
        }

        private static void LogTwoFingerDragGesture(Gesture<TwoFingerDragGesture> gesture)
        {
            string gestureName = "TwoFingerDrag";

            Debug.Log(gestureName + " gesture started.");

            gesture.onFinished += (s) =>
            {
                Debug.Log(gestureName + " gesture finished.");
            };
        }

        private static void LogPinchGesture(Gesture<PinchGesture> gesture)
        {
            string gestureName = "Pinch";

            Debug.Log(gestureName + " gesture started.");

            gesture.onFinished += (s) =>
            {
                Debug.Log(gestureName + " gesture finished.");
            };
        }

        private static void LogTwistGesture(Gesture<TwistGesture> gesture)
        {
            string gestureName = "Twist";

            Debug.Log(gestureName + " gesture started.");

            gesture.onFinished += (s) =>
            {
                Debug.Log(gestureName + " gesture finished.");
            };
        }
    }
}
