using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    public Text healthText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = GetComponent<Target>().health.ToString();
    }
}
