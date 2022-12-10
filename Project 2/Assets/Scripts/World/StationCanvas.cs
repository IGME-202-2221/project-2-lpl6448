using UnityEngine;
using UnityEngine.UI;

public class StationCanvas : MonoBehaviour
{
    public enum HoverState
    {
        None,
        Hovering,
        Pressing
    }

    public Station station;

    public Canvas canvas;

    public Image itemIconImage;

    public Sprite waitingForItemIcon;

    public MeshRenderer circleIndicator;

    public float circleIndicatorSpring = 10;

    public float circleIndicatorDamper = 2;

    public float noneCircleIndicatorSize = 0;

    public float hoveringCircleIndicatorSize = 0;

    public float pressingCircleIndicatorSize = 0;

    public Material[] circleIndicatorMaterials;

    private float currentCircleIndicatorSize = 1;

    public float goalCircleIndicatorSize = 1;

    private float circleIndicatorVelocity = 0;

    private HoverState currentHoverState = HoverState.None;

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

    public void UpdateHoverState(HoverState hoverState)
    {
        if (hoverState != currentHoverState)
        {
            currentHoverState = hoverState;
            UpdateIndicatorSize();
        }
    }

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

    public void EnableCircleIndicator(int material)
    {
        circleIndicator.sharedMaterial = circleIndicatorMaterials[material];
        UpdateIndicatorSize();
    }

    public void DisableCircleIndicator()
    {
        goalCircleIndicatorSize = 0;
    }

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