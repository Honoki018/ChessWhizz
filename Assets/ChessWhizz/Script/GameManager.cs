using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Network{
	public class GameManager : NetworkBehaviour {
		[SyncVar]
		public string colorState;
		[SyncVar]
		string _message = "";
		[SyncVar]
		public string winner;
		[SyncVar]
		public int downTime = 10;
		[SyncVar]
		public int lastTime = 0;
		public SqliteDatabase database;

		void Start(){
			if(!isServer)
				return;

			colorStateChange();
			database = new SqliteDatabase("testDB.db");
			database.ExecuteNonQuery("update Question set status = 0");
		}

		void OnGUI(){
			if(winner == ""){
				GUI.skin.box.fontSize = (int)Screen.width / 35;
				GUI.skin.box.alignment = TextAnchor.MiddleCenter;
				GUI.Box(new Rect(Screen.width*0.35f, Screen.height*0.05f, Screen.width*0.3f, Screen.height*0.1f),_message);
			}
			else{
				countDownTimer();
				GUI.skin.box.fontSize = (int)Screen.width / 30;
				GUI.skin.box.alignment = TextAnchor.MiddleCenter;
				GUI.Box(new Rect(Screen.width*0.3f, Screen.height*0.45f, Screen.width*0.5f, Screen.width*0.15f),"WINNER "+winner+"!\nServer is Shutting Down in "+downTime);
				if(downTime <= 0)
					serverShotdown();
			}
		}

		[ServerCallback]
		public void colorStateChange(){
			if(colorState == "White"){
				colorState = "Black";
				_message = colorState+"'s turn";
			}
			else{
				colorState = "White";
				_message = colorState+"'s turn";
			}
		}

		[ServerCallback]
		public void serverShotdown(){
			NetworkServer.Shutdown();
			Network.LobbyManager.Shutdown();
			MasterServer.UnregisterHost();
		}

		[ServerCallback]
		void countDownTimer(){
			if((int)Time.time != lastTime){
				downTime--;
				lastTime = (int)Time.time;
			}
		}
	}
}
