using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject BTN_Menu;
    [SerializeField] private GameObject BTN_CloseMenu;

    [SerializeField] private GameObject darkMenuPanel;

    void Start()
    {
        
    }

    public void openMenu()
    { 
        darkMenuPanel.SetActive(true);
        BTN_Menu.SetActive(false);

        Time.timeScale = 0f;
    }

    public void closeMenu()
    {
        darkMenuPanel.SetActive(false);
        BTN_Menu.SetActive(true);
        Time.timeScale = 1f;
    }

}
