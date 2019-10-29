using UnityEngine;

namespace GatheringChess
{
    public class CameraController : MonoBehaviour
    {
        void Start()
        {
            var camera = GetComponent<Camera>();

            // set camera size
            if (camera.aspect < 1f)
                camera.orthographicSize = 4f / camera.aspect;
            else
                camera.orthographicSize = 4f;
        }
    }
}
