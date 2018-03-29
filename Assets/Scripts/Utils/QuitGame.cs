using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuitGame : MonoBehaviour {

    private Button button;

	// Use this for initialization
	void Start () {
        button = this.GetComponent<Button>();
        if(button)
        {
            button.onClick.AddListener(delegate () {
                Application.Quit();
            });
        }
    }
}
