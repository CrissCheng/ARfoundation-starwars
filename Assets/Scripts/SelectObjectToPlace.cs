using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SelectObjectToPlace : MonoBehaviour
{
    ARRaycastManager arRaycastManager;
    bool planeIsDetected;
    Pose placementPose;
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    public GameObject cube;
    public GameObject sphere;
    public GameObject cubeButton;
    public GameObject sphereButton;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

        objectToPlace = cube;

        cubeButton.GetComponent<Button>().onClick.AddListener(SwitchObjectToPlace);
        sphereButton.GetComponent<Button>().onClick.AddListener(SwitchObjectToPlace);
    }


    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // Input: https://docs.unity3d.com/ScriptReference/Input.html
        if (planeIsDetected && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject(Input.GetTouch(0));
        }
    }

    private void UpdatePlacementPose()
    {
        // ViewportToScreenPoint: https://docs.unity3d.com/ScriptReference/Camera.ViewportToScreenPoint.html 
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        // ARRaycastManager: https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@2.1/api/UnityEngine.XR.ARFoundation.ARRaycastManager.html
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        planeIsDetected = hits.Count > 0;

        if (planeIsDetected)
        {
            // Pose: https://docs.unity3d.com/ScriptReference/Pose.html
            placementPose = hits[0].pose;
            // Transform.forward: https://docs.unity3d.com/ScriptReference/Transform-forward.html
            Vector3 cameraForward = Camera.main.transform.forward;
            // Vector3.normalized: https://docs.unity3d.com/ScriptReference/Vector3-normalized.html
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            // Quaternion.LookRotation: https://docs.unity3d.com/ScriptReference/Quaternion.LookRotation.html
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (planeIsDetected)
        {
            // GameObject.SetActive: https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }


    private void PlaceObject(Touch touch)
    {
        if (!IsPointerOverUIObject(touch))
        {
            // Instantiate: https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        }

    }

    private void SwitchObjectToPlace()
    {
        if (objectToPlace == cube)
        {
            objectToPlace = sphere;
        }
        else
        {
            objectToPlace = cube;
        }
    }

    // Helper function
    public static bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
