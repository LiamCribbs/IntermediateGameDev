using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pigeon.Math;

public class CameraEffectors : MonoBehaviour
{
    public static CameraEffectors instance;

    public float cameraFocusSpeed = 2f;

    public List<CameraBounds> bounds;
    public List<CameraMagnet> magnets;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Returns the new camera position
    /// </summary>
    public Vector2 HandleEffectors(Camera camera, Vector2 camPos)
    {
        float width = (float)Screen.width / Screen.height * camera.orthographicSize;
        Vector2 camSize = new Vector2(width, width * Screen.height / Screen.width);

        for (int i = 0; i < magnets.Count; i++)
        {
            var magnet = magnets[i];
            Vector2 boundPos = magnet.transform.position;

            if (camPos.x < boundPos.x + magnet.size.x && camPos.x > boundPos.x - magnet.size.x &&
                camPos.y < boundPos.y + magnet.size.y && camPos.y > boundPos.y - magnet.size.y)
            {
                bool horizontal = magnet.distanceTestMode == CameraMagnet.DistanceTestMode.Horizontal;
                float distance = horizontal ? Mathf.Abs(camPos.x - boundPos.x) : Mathf.Abs(camPos.y - boundPos.y);

                distance = magnet.pullStrength.Evaluate(distance / (horizontal ? magnet.size.x : magnet.size.y));
                camPos = Vector2.LerpUnclamped(boundPos, camPos, distance);

                break;
            }
        }

        for (int i = 0; i < bounds.Count; i++)
        {
            var bound = bounds[i];
            Vector2 boundPos = bound.transform.position;

            // X constraints
            if (camPos.x + camSize.x > boundPos.x + bound.size.x)
            {
                camPos.x = boundPos.x + bound.size.x - camSize.x;
            }
            else if (camPos.x - camSize.x < boundPos.x - bound.size.x)
            {
                camPos.x = boundPos.x - bound.size.x + camSize.x;
            }

            // Y constraints
            //this.Print(camPos.y - camSize.y, boundPos.y - bound.size.y);
            if (camPos.y + camSize.y > boundPos.y + bound.size.y)
            {
                camPos.y = boundPos.y + bound.size.y - camSize.y;
            }
            else if (camPos.y - camSize.y < boundPos.y - bound.size.y)
            {
                camPos.y = boundPos.y - bound.size.y + camSize.y;
            }

            //camera.transform.position = camPos;
            //camPos = camera.transform.position;
        }

        return camPos;
    }
}