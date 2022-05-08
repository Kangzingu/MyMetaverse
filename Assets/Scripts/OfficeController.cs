using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;

public class OfficeController : MonoBehaviour
{
    public GameObject canvas;
    public GameObject office;
    public ARSessionOrigin arSessionOrigin;
    private ARRaycastManager raycastManager;
    private Camera cam;

    private bool isPlaced;
    private Touch touch;
    // Start is called before the first frame update
    void Start()
    {
        raycastManager=arSessionOrigin.GetComponent<ARRaycastManager>();
        cam = arSessionOrigin.camera;
        
        SetTransparency(0.5f);
        
        canvas.SetActive(false);
        canvas.GetComponentInChildren<SampleWebView>().webViewObject.enabled = false;
        canvas.GetComponentInChildren<SampleWebView>().webViewObject.SetVisibility(false);
        isPlaced = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlaceOffice();
    }

    private void PlaceOffice()
    {
        if (Input.touchCount > 0)
        { // 터치 중
            touch = Input.GetTouch(0);
            if(touch.tapCount >= 2)
            { // 두번 탭했음
                if (isPlaced == true)
                { // 이미 배치됨
                    Ray ray = cam.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.CompareTag("MyPCScreen"))
                        {
                            canvas.SetActive(true);
                            canvas.GetComponentInChildren<SampleWebView>().webViewObject.enabled = true;
                            canvas.GetComponentInChildren<SampleWebView>().webViewObject.SetVisibility(true);
                        }
                        else
                        {
                            isPlaced = false;
                            SetTransparency(0.5f);
                        }
                    }
                    else
                    {
                        isPlaced = false;
                        SetTransparency(0.5f);
                    }
                }
                else // isPlaced==false
                { // 아직 배치 안됨
                    isPlaced = true;
                    SetTransparency(1f);
                }
            }
            else // touch.tapCount <= 1
            { // 탭하지 않음
                if (isPlaced == false)
                {
                    List<ARRaycastHit> hits = new List<ARRaycastHit>();
                    if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
                    {
                        Pose hitPose = hits[0].pose;
                        office.transform.SetPositionAndRotation(hitPose.position, office.transform.rotation);
                        Vector3 pos = cam.transform.position;
                        pos.y = hitPose.position.y;
                        Vector3 relativePos = pos - hitPose.position;
                        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                        office.transform.rotation = rotation;
                    }
                }
            }
        }
    }
    private void SetTransparency(float alpha)
    {
        Renderer[] childrenRenderer = office.GetComponentsInChildren<Renderer>();
        Color color;

        if (alpha == 1f)
        { // 불투명하게
            foreach (Renderer child in childrenRenderer)
            {
                foreach (Material material in child.materials)
                {
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
            }
        }
        else
        { // 투명하게
            foreach (Renderer child in childrenRenderer)
            {
                foreach (Material material in child.materials)
                {
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
            }
        }
    }
}
