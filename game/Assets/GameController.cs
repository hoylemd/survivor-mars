﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public GameObject currentRobot = null;
	public GameObject oldRobot = null;
	public GameObject[] Robots;
	public GameObject[] RobotButtons;
	public GameObject[] SpawnPoints;
	public GameObject CamToggleButton;
	public GameObject click;

	public Camera Surface;
	public Camera Surface_PIP;
	public bool CamToggleState = false;
	Camera currentRobotCam;

	//public Transform target;

	// Use this for initialization
	void Start () {

		


		for(int i = 0; i < Robots.Length; i++)
		{
			if (Robots[i].gameObject.activeSelf == false){
				if (i < RobotButtons.Length && RobotButtons[i] != null) {
					RobotButtons[i].SetActive(false);
				}
			}
			else {
				Robots[i].transform.transform.GetChild (4).gameObject.GetComponent<TextMesh>().text = (i+1).ToString();
			}
		}
	}
	
	void Update () {
		
		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()) {
			
			RaycastHit hitInfo = new RaycastHit ();
			
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo)) {

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);





				Physics.Raycast (ray, out hitInfo, Mathf.Infinity);
				
				Debug.DrawLine (ray.origin, hitInfo.point);
				
				Debug.Log (hitInfo.collider.tag);

				if (hitInfo.collider.tag == "Robot_surface")	{

					if (hitInfo.collider.gameObject != currentRobot && currentRobot != null){
						DeselectRobot();
					}
					if (hitInfo.collider.gameObject != currentRobot){
						SwitchRobot(hitInfo.transform.gameObject);
					}
				}
				
				if ((hitInfo.collider.tag == "TubeEntrance"|| hitInfo.collider.tag == "Building") && currentRobot.GetComponent<Robot_surfaceMove>().inTube == false && currentRobot != null) {

					if (currentRobot != null) {
						//currentRobot.GetComponent<Robot_surfaceMove>().dest = hitInfo.collider.transform;
						//Instantiate(click, hitInfo.point, Quaternion.identity);
						currentRobot.GetComponent<Robot_surfaceMove>().target = hitInfo.point;
						currentRobot.GetComponent<Robot_surfaceMove>().moving = true;
					} else {
						DeselectRobot();
					}
				}
					
				if (hitInfo.collider.tag == "CaveFloor")	{

					if (currentRobot != null && currentRobot.GetComponent<Robot_surfaceMove>().inTube == true) {
						Instantiate(click, hitInfo.point, Quaternion.identity);
						currentRobot.GetComponent<Robot_surfaceMove>().target = hitInfo.point;
						currentRobot.GetComponent<Robot_surfaceMove>().moving = true;
					} else {
						DeselectRobot();
					}
					
				} else if (hitInfo.collider.tag == "Mining-Ore") {
					if (currentRobot != null) {
						currentRobot.GetComponent<MineSequence>().Mine(hitInfo.point);
						currentRobot.GetComponent<ResourceOutput>().type = ResourceType.Ore;
						currentRobot.GetComponent<ResourceConverter>().online = true;

					}
				
				} else if (hitInfo.collider.tag == "Mining-Ice") {
					if (currentRobot != null) {
						currentRobot.GetComponent<MineSequence>().Mine(hitInfo.point);
						currentRobot.GetComponent<ResourceOutput>().type = ResourceType.Ice;
						currentRobot.GetComponent<ResourceConverter>().online = true;

					}
				}
				
				if (hitInfo.collider.tag == "GamePlane" )	{
						DeselectRobot();
				}
			}
		}
	}

	public void EnterTube(int tube, GameObject robot){
			CamToggleButton.SetActive(true);
			robot.transform.position = SpawnPoints[tube].transform.position;
			SwitchRobot (robot);

	}







	public void SelectRobot (int num){ //legacy but didnt want to rename, wrapper for UI
		SwitchRobot (Robots[num]);
	}

	public void SwitchRobot(GameObject newRobot){
		DeselectRobot ();
		currentRobot = newRobot;
		if (currentRobot != null) {
			currentRobot.transform.GetChild (3).gameObject.SetActive (true);				//Turn ON Object Highlighter

			if (currentRobot.GetComponent<Robot_surfaceMove> ().inTube == true) {			//If IN Tube
					CamToggleButton.SetActive (true);										//Activate Camera Button
					currentRobot.transform.GetChild (1).gameObject.SetActive (true);		//Turn on PIP
			}
		}
	}

	public void DeselectRobot(){
		if (currentRobot != null) {

			//if (currentRobot.GetComponent<Robot_surfaceMove> ().inTube == true) {			//If IN Tube
				CamReset();
			//}

				currentRobot.transform.GetChild (1).gameObject.SetActive (false); 	//Turn off PIP
				//currentRobot.transform.GetChild (2).gameObject.SetActive (false); //Turn off Fullscreen Cam
				currentRobot.transform.GetChild (3).gameObject.SetActive (false); 	//Turn off Object Highlighter
				currentRobot = null;											  	//Unset current robot
				//CamToggleButton.SetActive(false);
		}
	}



	public void CamToggler(){
		Debug.Log ("CamToggler");
		//CamToggleState = !CamToggleState;
		//CamToggleState True = above ground False = Below ground
		//CameraState: 
		// 0 = Above Ground
		// 1 = Below Ground



		//if (CamToggleState == true ) {
		if (Surface.tag == "MainCamera"){
		//if (currentRobot.GetComponent<Robot_surfaceMove>().inTube == true) {
				currentRobot.transform.GetChild (2).gameObject.SetActive (true);	//Turn ON fullscreen Robot Cam
				Surface_PIP.gameObject.SetActive(true);								//Turn ON Surface PIP Cam
				Surface.tag = "MainCamera_Bak";                                     //Unassign Main Cam
				Surface.camera.enabled = false;                                     //Turn OFF Main Cam
				//currentRobot.transform.GetChild (2).tag = "MainCamera";
		} 


		//else if (currentRobot.GetComponent<Robot_surfaceMove>().inTube == false){
		else { 
				currentRobot.transform.GetChild (2).gameObject.SetActive (false);	//Turn off Fullscreen Robot Cam
				Surface_PIP.gameObject.SetActive(false);							//Turn off Surface PIP
				Surface.tag = "MainCamera";											//Retag main camer
				Surface.camera.enabled = true;										//Enabled surface camera
		}

	}

	public void CamReset() {
		currentRobot.transform.GetChild (2).gameObject.SetActive (false);	//Turn off Fullscreen Robot Cam
		Surface_PIP.gameObject.SetActive(false);							//Turn off Surface PIP
		Surface.tag = "MainCamera";											//Retag main camer
		Surface.camera.enabled = true;										//Enabled surface camera
	}




}
