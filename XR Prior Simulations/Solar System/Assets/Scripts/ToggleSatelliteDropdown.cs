using UnityEngine;
using TMPro;

public class ToggleDropdown : MonoBehaviour
{
    public GameObject dropdownObject;

    public void ToggleVisibility()
    {
        if (dropdownObject != null)
        {
            bool isActive = dropdownObject.activeSelf;
            dropdownObject.SetActive(!isActive);
        }
    }
}