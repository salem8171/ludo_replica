using UnityEngine;

public class Credits : MonoBehaviour {

    public ScreenFader fader;

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Menu();
        }
    }

    private void Menu()
    {
        fader.FadeTo("MainMenu");
    }
}
