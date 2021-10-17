using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMarker : MonoBehaviour
{
    #region constants
    const float DEFAULT_FADE_TIME = 2;
    const float DEFAULT_STARTING_FADE = .9f;
    const float DEFAULT_GROUND_CLEARANCE = -0.02f;
    const float DEFAULT_UNIT_CLEARANCE = 0.24f;
    #endregion
    #region public fields
    public SpriteRenderer Image;
    public GameObject ImageContainer;
    public Camera MainCamera;
    #endregion
    #region private fields

    #endregion
    #region properties
    public float FadeTime { get; set; } = DEFAULT_FADE_TIME;
    public float StartingFade { get; set; } = DEFAULT_STARTING_FADE;
    public bool ShouldFaceCamera { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        if(MainCamera == null)
        {
            MainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //rotate marker to face the camera
        //the image container has the offset, so it should display in between the unit and the camera
        if (ShouldFaceCamera)
        {
            //transform.rotation = MainCamera.transform.rotation * transform.parent.rotation;
            transform.forward = MainCamera.transform.position - transform.position;
        }
        //fade out
        Image.color = FadeColor(GetFade(Time.deltaTime));
        //remove
        if(Image.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region public methods
    public ActionMarker Instantiate(Sprite sprite, Transform transform, Vector3 position, bool faceCamera)
    {
        var marker = Instantiate(this, transform);
        marker.Image.sprite = sprite;
        marker.Image.color = marker.FadeColor(StartingFade);
        
        marker.ShouldFaceCamera = faceCamera;
        if (!faceCamera)
        {
            marker.transform.position = position;
            marker.transform.forward = Vector3.down;
            marker.ImageContainer.transform.localPosition = new Vector3(0, 0, DEFAULT_GROUND_CLEARANCE);
        }
        else
        {
            marker.ImageContainer.transform.localPosition = new Vector3(0, 0, DEFAULT_UNIT_CLEARANCE);
        }

        return marker;
    }
    #endregion
    #region private fields
    private Color FadeColor(float fade)
    {

        return new Color(Image.color.r, Image.color.g, Image.color.b, fade);
    }
    private float GetFade(float time)
    {

        return Image.color.a - time / FadeTime * StartingFade;
    }
    #endregion

}
