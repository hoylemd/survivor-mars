using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class closeUIElement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Time.timeScale =0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClicked()
	{
		//button.gameObject.SetActive (false);
		Time.timeScale =1;
		Destroy (gameObject);
	}
}
