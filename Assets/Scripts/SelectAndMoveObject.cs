using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SelectAndMoveObject : MonoBehaviour
{
    ARRaycastManager arRaycastManager;
    bool planeIsDetected;
    Pose placementPose;
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    public GameObject cube;
    public GameObject object2;
    public GameObject object3;
    public GameObject cubeButton;
    public GameObject Button2;
    public GameObject Button3;

    private GameObject placedObject;
    bool isPlaced;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

        objectToPlace = cube;
        isPlaced = false;

        cubeButton.GetComponent<Button>().onClick.AddListener(delegate { SwitchObjectToPlace(cubeButton.name); });
        Button2.GetComponent<Button>().onClick.AddListener(delegate { SwitchObjectToPlace(Button2.name); });
        Button3.GetComponent<Button>().onClick.AddListener(delegate { SwitchObjectToPlace(Button3.name); });

    }


    void Update()
    {
        UpdatePlacementPose();
        UpdateUI();
        UpdatePlacementIndicator();

        // Input: https://docs.unity3d.com/ScriptReference/Input.html
        if (planeIsDetected && Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            if (!isPlaced)
            {
                PlaceObject(Input.GetTouch(0));
                isPlaced = true;
            }
            else
            {
                MoveObject(Input.GetTouch(0));
            }

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
        if (planeIsDetected && !isPlaced)
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
            placedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        }

    }

    private void MoveObject(Touch touch)
    {
        if (!IsPointerOverUIObject(touch))
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon);

            if (hits.Count > 0)
            {
                placedObject.transform.position = hits[0].pose.position;
                placedObject.transform.rotation = hits[0].pose.rotation;
            }
        }
    }

    private void SwitchObjectToPlace(string buttonName)
    {
        //if (objectToPlace == cube)
        //{
        //    Destroy(placedObject);
        //    isPlaced = false;
        //    objectToPlace = object2;
        //}
        //else if (objectToPlace == object2)
        //{
        //    Destroy(placedObject);
        //    isPlaced = false;
        //    objectToPlace = object3;
        //}
        //else
        //{
        //    Destroy(placedObject);
        //    isPlaced = false;
        //    objectToPlace = cube;
        //}
        switch (buttonName)
        {
            case "cubeButton":
                Destroy(placedObject);
                isPlaced = false;
                objectToPlace = cube;
                break;
            case "Button2":
                Destroy(placedObject);
                isPlaced = false;
                objectToPlace = object2;
                break;
            case "Button3":
                Destroy(placedObject);
                isPlaced = false;
                objectToPlace = object3;
                break;
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

    void UpdateUI()
    {
        if (planeIsDetected)
        {
            cubeButton.SetActive(true);
            Button2.SetActive(true);
            Button3.SetActive(true);
        }
        else
        {
            cubeButton.SetActive(false);
            Button2.SetActive(false);
            Button3.SetActive(false);
        }
    }

}
