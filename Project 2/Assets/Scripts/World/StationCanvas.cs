using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains UI information about a particular Station and an on-ground circle
/// indicator that provides information when in Selection Mode.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class StationCanvas : MonoBehaviour
{
    /// <summary>
    /// Contains information about the cursor's interaction with the Station
    /// None: The cursor is neither hovering nor clicking on this Station
    /// Hovering: The cursor is currently hovered over this Station
    /// Pressing: The cursor is clicking on this Station
    /// </summary>
    public enum HoverState
    {
        None,
        Hovering,
        Pressing
    }

    /// <summary>
    /// Station that this StationCanvas provides information about
    /// </summary>
    public Station station;

    /// <summary>
    /// Reference to the Camera-facing Canvas for this StationCanvas
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Image that displays the icon of the item that this Station is currently building
    /// </summary>
    public Image itemIconImage;

    /// <summary>
    /// Sprite to display in the itemIconImage when this Station is not building anything
    /// </summary>
    public Sprite waitingForItemIcon;

    /// <summary>
    /// Reference to the MeshRenderer that renders the circle indicator
    /// </summary>
    public MeshRenderer circleIndicator;

    /// <summary>
    /// Spring coefficient of the circle indicator radius
    /// </summary>
    public float circleIndicatorSpring = 10;

    /// <summary>
    /// Damper coefficient of the circle indicator radius
    /// </summary>
    public float circleIndicatorDamper = 2;

    /// <summary>
    /// Diameter of the circle indicator when the Station is not being hovered over nor pressed
    /// </summary>
    public float noneCircleIndicatorSize = 0;

    /// <summary>
    /// Diamater of the circle indicator when the Station is being hovered over
    /// </summary>
    public float hoveringCircleIndicatorSize = 0;

    /// <summary>
    /// Diamter of the circle indicator when the Station is being pressed
    /// </summary>
    public float pressingCircleIndicatorSize = 0;

    /// <summary>
    /// Array of 2 Materials applied to the circleIndicator (0: not accepting, 1: accepting)
    /// </summary>
    public Material[] circleIndicatorMaterials;

    /// <summary>
    /// Current diameter of the circle indicator
    /// </summary>
    private float currentCircleIndicatorSize = 1;

    /// <summary>
    /// Goal diameter of the circle indicator
    /// </summary>
    public float goalCircleIndicatorSize = 1;

    /// <summary>
    /// Current velocity of the circle indicator diameter
    /// </summary>
    private float circleIndicatorVelocity = 0;

    /// <summary>
    /// Current HoverState for this StationCanvas, containing information about the mouse
    /// cursor's interactions with the Station
    /// </summary>
    private HoverState currentHoverState = HoverState.None;

    /// <summary>
    /// Updates the itemIconImage based on the icon of the item given. If item is null,
    /// the StationCanvas displays the waitingForItemIcon.
    /// </summary>
    /// <param name="item">ItemType being built by the Station</param>
    public void SetItem(ItemType item)
    {
        if (item == null)
        {
            itemIconImage.sprite = waitingForItemIcon;
            itemIconImage.enabled = true;
        }
        else if (item.icon != null)
        {
            itemIconImage.sprite = item.icon;
            itemIconImage.enabled = true;
        }
        else
        {
            itemIconImage.enabled = false;
        }
    }

    /// <summary>
    /// Sets this StationCanvas's hoverState and updates the goalCircleIndicatorSize
    /// </summary>
    /// <param name="hoverState">New HoverState of the StationCanvas</param>
    public void UpdateHoverState(HoverState hoverState)
    {
        if (hoverState != currentHoverState)
        {
            currentHoverState = hoverState;
            UpdateIndicatorSize();
        }
    }

    /// <summary>
    /// Updates the goalCircleIndicatorSize based on the currentHoverState,
    /// using the values defined in the Inspector
    /// </summary>
    private void UpdateIndicatorSize()
    {
        switch (currentHoverState)
        {
            case HoverState.None:
                goalCircleIndicatorSize = noneCircleIndicatorSize;
                break;
            case HoverState.Hovering:
                goalCircleIndicatorSize = hoveringCircleIndicatorSize;
                break;
            case HoverState.Pressing:
                goalCircleIndicatorSize = pressingCircleIndicatorSize;
                break;
        }
    }

    /// <summary>
    /// Displays the circle indicator using the given material index and initializes the diameter.
    /// 0: not accepting
    /// 1: accepting
    /// </summary>
    /// <param name="material">Material index in circleIndicatorMaterials that is applied to the circleIndicator</param>
    public void EnableCircleIndicator(int material)
    {
        circleIndicator.sharedMaterial = circleIndicatorMaterials[material];
        UpdateIndicatorSize();
    }
    
    /// <summary>
    /// Deactivates the circle indicator, setting its goal diameter to 0
    /// </summary>
    public void DisableCircleIndicator()
    {
        goalCircleIndicatorSize = 0;
    }

    /// <summary>
    /// Every frame, faces the canvas toward the camera and updates the circle indicator's diameter
    /// </summary>
    private void LateUpdate()
    {
        if (canvas != null)
        {
            canvas.transform.forward = Camera.main.transform.forward;
        }

        if (circleIndicator != null)
        {
            circleIndicatorVelocity += (goalCircleIndicatorSize - currentCircleIndicatorSize) * circleIndicatorSpring * Time.deltaTime;
            circleIndicatorVelocity *= 1 - Mathf.Clamp01(circleIndicatorDamper * Time.deltaTime);
            currentCircleIndicatorSize += circleIndicatorVelocity * Time.deltaTime;
            if (currentCircleIndicatorSize < 0)
            {
                currentCircleIndicatorSize = 0;
                circleIndicatorVelocity = 0;
            }

            circleIndicator.transform.localScale = Vector3.one * currentCircleIndicatorSize;
        }
    }
}