using UnityEngine;
using UnityEngine.InputSystem;

public class GamePanel : MonoBehaviour
{
    public Animation indicatorAnimation;

    public GameObject tutorialText;

    public float minimumSelectionDistance = 4;

    private bool selectionMode = false;

    private ItemType selectingItem = null;

    public void EnableSelectionMode(ItemType item)
    {
        selectionMode = true;
        selectingItem = item;

        indicatorAnimation.Play();
        tutorialText.SetActive(true);

        UpdateCircleIndicatorColors();
    }

    public void DisableSelectionMode()
    {
        selectionMode = false;
        selectingItem = null;

        tutorialText.SetActive(false);

        UpdateCircleIndicatorColors();
    }

    public void UpdateCircleIndicatorColors()
    {
        foreach (Station station in WorldManager.Instance.stations)
        {
            if (station.stationCanvas != null)
            {
                if (selectingItem != null)
                {
                    station.stationCanvas.EnableCircleIndicator(station.CanAcceptUserItem(selectingItem) ? 1 : 0);
                }
                else
                {
                    station.stationCanvas.DisableCircleIndicator();
                }
            }
        }
    }

    private void ResetCircleIndicatorStates()
    {
        foreach (Station station in WorldManager.Instance.stations)
        {
            if (station.stationCanvas != null)
            {
                station.stationCanvas.UpdateHoverState(StationCanvas.HoverState.None);
            }
        }
    }

    private void Update()
    {
        if (selectionMode)
        {
            // Reset station hover states
            ResetCircleIndicatorStates();

            // Find which station (if any), the mouse is hovering over
            Plane plane = new Plane(Vector3.down, Vector3.zero);
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (plane.Raycast(mouseRay, out float t))
            {
                Vector3 mousePoint = mouseRay.GetPoint(t);

                Station closestStation = null;
                float closestDisSqr = float.MaxValue;
                foreach (Station station in WorldManager.Instance.stations)
                {
                    if (station.stationCanvas != null && station.CanAcceptUserItem(selectingItem))
                    {
                        float disSqr = Vector3.SqrMagnitude(station.transform.position - mousePoint);
                        if (disSqr < minimumSelectionDistance * minimumSelectionDistance && disSqr < closestDisSqr)
                        {
                            closestDisSqr = disSqr;
                            closestStation = station;
                        }
                    }
                }

                // Update or select the closest station
                if (closestStation != null)
                {
                    if (Mouse.current.leftButton.isPressed)
                    {
                        closestStation.stationCanvas.UpdateHoverState(StationCanvas.HoverState.Pressing);
                    }
                    else
                    {
                        closestStation.stationCanvas.UpdateHoverState(StationCanvas.HoverState.Hovering);
                    }

                    if (Mouse.current.leftButton.wasPressedThisFrame && !Mouse.current.rightButton.isPressed)
                    {
                        SelectStation(closestStation);
                    }
                }
            }
        }
    }

    private void SelectStation(Station station)
    {
        Taskmaster.Instance.BeginBuildingItem(selectingItem, station);

        DisableSelectionMode();
        UIManager.Instance.craftingPanel.ExitCraftMode();
    }
}