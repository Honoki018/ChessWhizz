using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace UnityStandardAssets.Network{
	public class CreateBoard : NetworkBehaviour {
		public Camera firstCam;
		public Camera whiteCam;
		public Camera blackCam;
		public GameObject tile1;
		public GameObject tile2;
		public GameObject wPawnPrefab;
		public GameObject bPawnPrefab;
		public GameObject wRookPrefab;
		public GameObject bRookPrefab;
		public GameObject wBishopPrefab;
		public GameObject bBishopPrefab;
		public GameObject wKnightPrefab;
		public GameObject bKnightPrefab;
		public GameObject wQueenPrefab;
		public GameObject bQueenPrefab;
		public GameObject wKingPrefab;
		public GameObject bKingPrefab;
		public GameObject managerPrefab;
		public string useCam = "Main";
		
		void Start () {
			if(!isServer)
				return;
			
			Cmd_createBoard();
		}

		void OnGUI(){
			GUI.skin.button.fontSize = (int)Screen.width / 35;
			GUI.skin.button.alignment = TextAnchor.MiddleCenter;
			if(GUI.Button(new Rect(Screen.width*0, Screen.height*0, Screen.width*0.2f, Screen.height*0.1f), useCam)){
				if(useCam == "Main"){
					firstCam.enabled = false;
					whiteCam.enabled = true;
					blackCam.enabled = false;
					useCam = "White";
				}
				else if(useCam == "White"){
					firstCam.enabled = false;
					whiteCam.enabled = false;
					blackCam.enabled = true;
					useCam = "Black";
				}
				else{
					firstCam.enabled = true;
					whiteCam.enabled = false;
					blackCam.enabled = false;
					useCam = "Main";
				}
			}
		}

		/*public void getColorCam(string color){
			if(color == "White"){
				firstCam.enabled = false;
				blackCam.enabled = false;
				whiteCam.enabled = true;
			}
			else if(color == "Black"){

			}
				
		}*/
		
		[Command]
		public void Cmd_createBoard(){
			for (int i = 0; i < 8; i++){
				for (int j = 0; j < 8; j++){
					Vector3 pos = new Vector3(i, 0, j);
					GameObject obj;
					if(i % 2 == 0){
						if(j % 2 == 0){
							obj = (GameObject)Instantiate(tile1, pos, Quaternion.identity);
							obj.name = i.ToString()+j.ToString();
						}
						else{
							obj = (GameObject)Instantiate(tile2, pos, Quaternion.identity);
							obj.name = i.ToString()+j.ToString();
						}
					}
					else{
						if(j % 2 == 0){
							obj = (GameObject)Instantiate(tile2, pos, Quaternion.identity);
							obj.name = i.ToString()+j.ToString();;
						}
						else{
							obj = (GameObject)Instantiate(tile1, pos, Quaternion.identity);
							obj.name = i.ToString()+j.ToString();
						}
					}
					
					NetworkServer.Spawn(obj);
				}
			}
			
			for(int i = 0; i < 8; i++){
				Vector3 pos = new Vector3(i, 2, 1);
				GameObject obj = (GameObject)Instantiate(wPawnPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
				obj.name = "Pawn";
				NetworkServer.Spawn(obj);
			}
			
			for(int i = 0; i < 8; i++){
				Vector3 pos = new Vector3(i, 2, 6);
				GameObject obj = (GameObject)Instantiate(bPawnPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
				obj.name = "Pawn";
				NetworkServer.Spawn(obj);
			}
			
			for(int i = 0; i < 8; i++){
				GameObject obj;
				Vector3 pos = new Vector3(i, 2, 0);
				if(i == 0 || i == 7){
					obj = (GameObject)Instantiate(wRookPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Rook";
				}
				else if(i == 1 || i == 6){
					obj = (GameObject)Instantiate(wKnightPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Knight";
				}
				else if(i == 2 || i == 5){
					obj = (GameObject)Instantiate(wBishopPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Bishop";
				}
				else if(i == 3){
					obj = (GameObject)Instantiate(wQueenPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Queen";
				}
				else{
					obj = (GameObject)Instantiate(wKingPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "King";
				}
				
				NetworkServer.Spawn(obj);
			}
			
			for(int i = 0; i < 8; i++){
				GameObject obj;
				Vector3 pos = new Vector3(i, 2, 7);
				if(i == 0 || i == 7){
					obj = (GameObject)Instantiate(bRookPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Rook";
				}
				else if(i == 1 || i == 6){
					obj = (GameObject)Instantiate(bKnightPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Knight";
				}
				else if(i == 2 || i == 5){
					obj = (GameObject)Instantiate(bBishopPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Bishop";
				}
				else if(i == 3){
					obj = (GameObject)Instantiate(bQueenPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "Queen";
				}
				else{
					obj = (GameObject)Instantiate(bKingPrefab, pos, Quaternion.AngleAxis(90, Vector3.left));
					obj.name = "King";
				}
				
				NetworkServer.Spawn(obj);
			}
			Vector3 managerPos = new Vector3(0,0,0);
			GameObject manager = (GameObject)Instantiate(managerPrefab, managerPos, Quaternion.identity);
			NetworkServer.Spawn(manager);
		}
	}
}
