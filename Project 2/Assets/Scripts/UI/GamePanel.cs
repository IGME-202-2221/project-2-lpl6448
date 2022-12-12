using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Represents the game panel, which lets users select a Station to craft an item at.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class GamePanel : MonoBehaviour
{
    /// <summary>
    /// Animation component that can flash indicators on screen to direct the user's
    /// attention toward the highlighted Stations
    /// </summary>
    public Animation indicatorAnimation;

    /// <summary>
    /// GameObject containing the instructions for how to select a Station, which is
    /// activated when Selection Mode is active
    /// </summary>
    public GameObject tutorialText;

    /// <summary>
    /// Maximum distance that the mouse cursor can be from a Station for it to be selected
    /// </summary>
    public float selectionDistance = 3;

    /// <summary>
    /// Whether the GamePanel is currently in Selection Mode
    /// </summary>
    private bool selectionMode = false;

    /// <summary>
    /// ItemType that a Station is being selected to craft, used for the Station indicator colors
    /// </summary>
    private ItemType selectingItem = null;

    /// <summary>
    /// Enables Selection Mode, considering the given item for crafting, and activates
    /// the Stations' circle indicators
    /// </summary>
    /// <param name="item">ItemType that may be crafted</param>
    public void EnableSelectionMode(ItemType item)
    {
        selectionMode = true;
        selectingItem = item;

        indicatorAnimation.Play();
        tutorialText.SetActive(true);

        UpdateCircleIndicatorColors();
    }

    /// <summary>
    /// Disables Selection Mode, resetting the GamePanel and disabling the Stations' circle indicators
    /// </summary>
    public void DisableSelectionMode()
    {
        selectionMode = false;
        selectingItem = null;

        tutorialText.SetActive(false);

        UpdateCircleIndicatorColors();
    }

    /// <summary>
    /// Updates all Stations' circle indicator colors depending on whether they can accept the selectingItem,
    /// or disables the circle indicators if Selection Mode is inactive
    /// </summary>
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

    /// <summary>
    /// Resets the hover states of all circle indicators (called every frame)
    /// </summary>
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

    /// <summary>
    /// Every frame that Selection Mode is active, updates hover states for the closest Station's
    /// circle indicator and selects the Station if the user left clicks
    /// </summary>
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
                        if (disSqr < selectionDistance * selectionDistance && disSqr < closestDisSqr)
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

    /// <summary>
    /// Selects the given Station, exiting Selection Mode and Craft Mode and telling the Taskmaster
    /// to begin building the selectingItem at the chosen Station
    /// </summary>
    /// <param name="station">Station to begin building the selectingItem at</param>
    private void SelectStation(Station station)
    {
        Taskmaster.Instance.BeginBuildingItem(selectingItem, station);

        DisableSelectionMode();
        UIManager.Instance.craftingPanel.ExitCraftMode();
    }
}