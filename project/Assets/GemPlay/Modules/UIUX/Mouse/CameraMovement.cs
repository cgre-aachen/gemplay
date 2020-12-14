using UnityEngine;

namespace GemPlay.Modules.UIUX.Mouse
{
    /// <summary>
    /// This class is mostly copied from https://answers.unity.com/questions/666905/in-game-camera-movement-like-editor.html  
    /// </summary>
    public class CameraMovement : MonoBehaviour
	{
        public GameObject cameraGameObject;

        public float lookSpeedH = 2f;

        public float lookSpeedV = 2f;

        public float zoomSpeed = 2f;

        public float dragSpeed = 6f;


        float yaw = 0f;

        float pitch = 0f;


		void Update()
		{
            // Look around with Right Mouse
            if (Input.GetMouseButton(1))
            {
                yaw += lookSpeedH * Input.GetAxis("Mouse X");

                pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

                cameraGameObject.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
            }


            // Drag camera around with Middle Mouse
            if (Input.GetMouseButton(2))
            {
                cameraGameObject.transform.Translate(
                    -Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed,
                    -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed,
                    0);
            }


            // Zoom in and out with Mouse Wheel
            cameraGameObject.transform.Translate(
                0,
                0,
                Input.GetAxis("Mouse ScrollWheel") * zoomSpeed,
                Space.Self);
		}
	}
}
