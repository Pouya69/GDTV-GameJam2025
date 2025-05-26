using UnityEngine;
using UnityEngine.UI;

public class TrackableUI : MonoBehaviour
{
    public enum ImageState
    {
        Enemy = 0,
        Objective = 1,
        Pickable = 2,
        GravityDevice = 3,
        GravityField = 4,
        ReTime = 5,
        Shot = 6
    }
    [Header("Tracking Paramters")]
    public Transform target; // The world object to track
    public RectTransform uiIcon; // The UI icon that follows the object
    private Camera cam;
    public Vector3 offset = new Vector3(0, 2, 0); // UI offset above the object

    [Header("Display UI")]
    public ImageState ImageType;
    public Sprite[] DisplayIcons;
    [SerializeField] private Image Image;
    [SerializeField] private Text DistTxt;
    [SerializeField] private bool TrackDistance;
    public void Awake()
    {
        Image.sprite = DisplayIcons[(int)ImageType];
        cam = Camera.main;
    }
    void Update()
    {
        if (target == null || cam == null) return;

        Vector3 worldPosition = target.position + offset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);

        bool isBehind = screenPos.z < 0;
        if (isBehind)
        {
            screenPos *= -1;
        }

        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

        bool offScreen = screenPos.x < 0 || screenPos.x > screenBounds.x ||
                         screenPos.y < 0 || screenPos.y > screenBounds.y || isBehind;

        if (offScreen)
        {
            screenPos.x = Mathf.Clamp(screenPos.x, 50, screenBounds.x - 50);
            screenPos.y = Mathf.Clamp(screenPos.y, 50, screenBounds.y - 50);
        }

        uiIcon.position = screenPos;
        if(DistTxt.gameObject.activeSelf != TrackDistance) DistTxt.gameObject.SetActive(TrackDistance);
        if(TrackDistance) DistTxt.text = $"{Vector3.Distance(target.position, cam.transform.position).ToString("f0")}m";
    }
}
