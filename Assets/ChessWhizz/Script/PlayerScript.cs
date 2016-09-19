using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Network{
	public class PlayerScript : NetworkBehaviour {
		[SyncVar]
		public GameObject selectedPiece;
		[SyncVar]
		private int selectedLevel;
		[SyncVar]
		public GameObject tileToMove;
		[SyncVar]
		public GameObject pieceToEat;
		[SyncVar]
		public string playerName;
		[SyncVar]
		public byte number;
		[SyncVar]
		public string pieceType;
		[SyncVar]
		public bool myQuiz = false;

		public SqliteDatabase database;
		[SyncVar]
		private string _problem;
		[SyncVar]
		private string _a;
		[SyncVar]
		private string _b;
		[SyncVar]
		private string _c;
		[SyncVar]
		private string _d;
		[SyncVar]
		private string _ans;
		List<GameObject> allPiece = new List<GameObject>();
		[SyncVar]
		private string selectedName = "";
		[SyncVar]
		private int timeDown;
		[SyncVar]
		private int timeLast;
		[SyncVar]
		private bool wrongBool = false;

		void Start(){
			if(isLocalPlayer){
				Screen.sleepTimeout = SleepTimeout.NeverSleep;
				Cmd_getType(number.ToString());
			}
		}

		void OnGUI(){
			if(!isLocalPlayer)
				return;

			if(wrongBool == true){
				if(GUI.Button(new Rect(Screen.width*0.2f, Screen.height*0.3f, Screen.width*0.6f, Screen.height*0.15f), "End of Turn: Correct Answer - "+_ans)){
					Cmd_wrongBoolFalse();
					Cmd_playerChange();
				}
			}

			if(myQuiz == true){
				Cmd_downTimer();
				GUI.skin.button.fontSize = (int)Screen.width / 40;
				GUI.skin.button.alignment = TextAnchor.MiddleCenter;
				GUI.skin.button.wordWrap = true;
				GUI.skin.box.fontSize = (int)Screen.width / 30;
				GUI.skin.box.alignment = TextAnchor.MiddleCenter;
				GUI.skin.box.wordWrap = true;
				
				GUI.Box(new Rect(Screen.width*.05f, Screen.height*.05f, Screen.width*.9f, Screen.height*.9f),timeDown.ToString());
				GUI.Box(new Rect(Screen.width*.10f, Screen.height*.10f, Screen.width*.80f, Screen.height*.50f),_problem);

				if (GUI.Button(new Rect(Screen.width*.10f, Screen.height*.65f, Screen.width*.35f, Screen.height*.10f),_a)){
					if(_a == _ans){
						Cmd_myQuizChange();
						Cmd_eatEvent(pieceToEat);
					}
					else {
						Cmd_wrongBoolChange();
					}
				}
				if (GUI.Button(new Rect(Screen.width*.55f, Screen.height*.65f, Screen.width*.35f, Screen.height*.10f),_b)){
					if(_b == _ans){
						Cmd_myQuizChange();
						Cmd_eatEvent(pieceToEat);
					}
					else {
						Cmd_wrongBoolChange();
					}
				}
				if (GUI.Button(new Rect(Screen.width*.10f, Screen.height*.80f, Screen.width*.35f, Screen.height*.10f),_c)){
					if(_c == _ans){
						Cmd_myQuizChange();
						Cmd_eatEvent(pieceToEat);
					}
					else {
						Cmd_wrongBoolChange();
					}
				}
				if (GUI.Button(new Rect(Screen.width*.55f, Screen.height*.80f, Screen.width*.35f, Screen.height*.10f),_d)){
					if(_d == _ans){
						Cmd_myQuizChange();
						Cmd_eatEvent(pieceToEat);
					}
					else {
						Cmd_wrongBoolChange();
					}
				}
			}
			else{
				GUI.skin.box.fontSize = (int)Screen.width / 35;
				GUI.skin.box.alignment = TextAnchor.MiddleCenter;
				GUI.Box(new Rect(Screen.width*0.8f, Screen.height*0, Screen.width*0.2f, Screen.height*0.1f), pieceType+" : "+selectedName);
			}
		}

		void Update(){
			if(!isLocalPlayer)
				return;

			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
			//if(Input.GetMouseButtonDown(0)){
				if(myQuiz != true && wrongBool != true){
					GameManager gameManager = GameObject.Find("netGameManager(Clone)").GetComponent<GameManager>();
					if(gameManager.colorState == pieceType){
						Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
						//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						if(Physics.Raycast(ray, out hit)){
							if(hit.collider != null){
								if(hit.collider.gameObject.tag != "Tile" && hit.collider.gameObject.tag == pieceType){
									if(selectedPiece == null)
										Cmd_selectPiece(hit.collider.gameObject);
									else
										Cmd_selectPiece(hit.collider.gameObject);
								}
								if(selectedPiece != null && hit.collider.gameObject.tag == "Tile"){
									Cmd_selectTile(hit.collider.gameObject);
								}
								if(selectedPiece != null && hit.collider.gameObject.tag != "Tile" && hit.collider.gameObject.tag != pieceType){
									pieceToEat = hit.collider.gameObject;
									if(pieceToEat.GetComponent<PieceProperties>().pieceLevel > selectedLevel)
										Cmd_getQuiz();
									else
										Cmd_eatEvent(pieceToEat);
								}
							}
						}
					}
				}
			}
		}

		void FixedUpdate(){
			if(NetworkServer.active){
				if(tileToMove){
					Cmd_MovePiece();
				}
			}
		}

		[Command]
		public void Cmd_wrongBoolChange(){
			wrongBool = true;
			myQuiz = false;
		}

		[Command]
		public void Cmd_wrongBoolFalse(){
			wrongBool = false;
		}

		[Command]
		public void Cmd_myQuizChange(){
			myQuiz = false;
			wrongBool = false;
		}

		[Command]
		public void Cmd_playerChange(){
			playerChange();
		}

		[Command]
		public void Cmd_getQuiz(){
			getData();
			timeDown = 15;
			myQuiz = true;
		}

		[Command]
		public void Cmd_downTimer(){
			if(timeDown >=0){
				if((int)Time.time != timeLast){
					timeDown--;
					timeLast = (int)Time.time;
					if(timeDown <= 0){
						Cmd_wrongBoolChange();
					}
				}
			}
		}

		public void getData(){
			database = new SqliteDatabase("testDB.db");
			var data = database.ExecuteQuery("select * from Question where status = 0");
			int indx = Random.Range(0, data.Rows.Count);
			_problem = data.Rows[indx]["prob"].ToString();
			_a = data.Rows[indx]["a"].ToString();
			_b = data.Rows[indx]["b"].ToString();
			_c = data.Rows[indx]["c"].ToString();
			_d = data.Rows[indx]["d"].ToString();
			_ans = data.Rows[indx]["ans"].ToString();
			database.ExecuteNonQuery("update Question set status = 1 where prob = '"+_problem+"'");
		}

		[Command]
		public void Cmd_getType(string num){
			if(num == "0")
				pieceType = "White";
			else
				pieceType = "Black";
		}

		[Command]
		public void Cmd_eatEvent(GameObject piece){
			PieceProperties getPiece = piece.GetComponent<PieceProperties>();//check if the variable canEat is true, if it is. eat it!
			if(getPiece.canEat == true){
				if(piece.name == "King"){
					tileToMove = getPiece.location;
					NetworkServer.Destroy(piece);
					GameManager gameManager = GameObject.Find("netGameManager(Clone)").GetComponent<GameManager>();
					gameManager.winner = playerName;
				}
				else{
					tileToMove = getPiece.location;
					NetworkServer.Destroy(piece);
					//NetworkServer.UnSpawn(piece);
				}
			}
		}

		[Command]
		public void Cmd_MovePiece(){
			Vector3 moveTo = new Vector3(tileToMove.transform.position.x, 0.3f, tileToMove.transform.position.z);
			selectedPiece.transform.position = Vector3.MoveTowards(selectedPiece.transform.position, moveTo, 0.1f);
			if(selectedPiece.transform.position == moveTo){
				playerChange();
			}
		}

		[Command]
		public void Cmd_selectPiece(GameObject piece){
			tileReset();
			allPiece = new List<GameObject>();
			allPiece.AddRange(GameObject.FindGameObjectsWithTag("White"));
			allPiece.AddRange(GameObject.FindGameObjectsWithTag("Black"));
			for (int i = 0; i < allPiece.Count; i++){
				if(allPiece[i].GetComponent<PieceProperties>()){
					allPiece[i].GetComponent<PieceProperties>().canEat = false;
				}
			}
			selectedPiece = piece;
			selectedName = selectedPiece.name;
			selectedLevel = piece.GetComponent<PieceProperties>().pieceLevel;
			string loc = piece.GetComponent<PieceProperties>().location.name;
			vmCheckLocation(loc);
		}
		
		[Command]
		public void Cmd_selectTile(GameObject tile){
			if(tile.GetComponent<TileProperties>().canClick == true)
				tileToMove = tile;
			else
				selectedPiece = null;
		}

		public void playerChange(){
			selectedName = "";
			selectedPiece = null;
			tileToMove = null;
			selectedLevel = 0;
			pieceToEat = null;
			GameManager gameManager = GameObject.Find("netGameManager(Clone)").GetComponent<GameManager>();
			gameManager.colorStateChange();
		}

		public void tileReset(){
			for(int i = 0; i < 8; i++){
				for(int j = 0; j < 8; j++){
					TileProperties tile = GameObject.Find(i.ToString()+j.ToString()).GetComponent<TileProperties>();
					tile.canClick = false;
				}
			}
		}

		public void vmCheckLocation(string strLoc){
			int intLoc;
			int x;
			int z;
			
			int.TryParse(strLoc, out intLoc);
			
			z = intLoc % 10;
			x = (intLoc / 10) % 10;
			vmCheckPiece(x, z);
		}

		public void vmCheckPiece(int x, int z){
			if(selectedPiece.name == "Pawn"){
				if(selectedPiece.tag == "Black"){
					vmBlackPawn(x, z);
				}
				else{
					vmWhitePawn(x, z);
				}
			}
			else if(selectedPiece.name == "Rook"){
				vmRook(x, z);
			}
			else if(selectedPiece.name == "Bishop"){
				vmBishop(x, z);
			}
			else if(selectedPiece.name == "Queen"){
				vmRook(x, z);
				vmBishop(x, z);
			}
			else if(selectedPiece.name == "King"){
				vmKing(x, z);
			}
			else if(selectedPiece.name == "Knight"){
				vmKnight(x, z);
			}
		}

		public void _piecesToEat(GameObject piece){ //set the variable canEat true
			PieceProperties pp = piece.GetComponent<PieceProperties>();
			pp.canEat = true;
		}

		public void vmBlackPawn(int x, int z){
			if(z == 6){
				for (int i = 1; i < 3; i++){
					TileProperties tile = GameObject.Find(x.ToString()+(z-i).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else
						break;
				}
			}
			else{
				if(z - 1 >= 0){
					TileProperties tile = GameObject.Find(x.ToString()+(z-1).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
				}
			}

			if(z-1>=0){
				if(x-1>=0){
					TileProperties tile1 = GameObject.Find((x-1).ToString()+(z-1).ToString()).GetComponent<TileProperties>();
					if(tile1.useBy)
						_piecesToEat(tile1.useBy);
				}
				
				if(x+1<8){
					TileProperties tile2 = GameObject.Find((x+1).ToString()+(z-1).ToString()).GetComponent<TileProperties>();
					if(tile2.useBy)
						_piecesToEat(tile2.useBy);
				}
			}
		}

		public void vmWhitePawn(int x, int z){
			if(z == 1){
				for (int i = 1; i < 3; i++){
					TileProperties tile = GameObject.Find(x.ToString()+(z+i).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else
						break;
				}
			}
			else{
				if(z + 1 < 8){
					TileProperties tile = GameObject.Find(x.ToString()+(z+1).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
				}
			}
			if(z+1<8){
				if(x-1>=0){
					TileProperties tile1 = GameObject.Find((x-1).ToString()+(z+1).ToString()).GetComponent<TileProperties>();
					if(tile1.useBy)
						_piecesToEat(tile1.useBy);
				}
				if(x+1<8){
					TileProperties tile2 = GameObject.Find((x+1).ToString()+(z+1).ToString()).GetComponent<TileProperties>();
					if(tile2.useBy)
						_piecesToEat(tile2.useBy);
				}
			}
		}

		public void vmRook(int x, int z){
			for (int i = z+1; i < 8 ; i++){
				TileProperties tile = GameObject.Find(x.ToString()+i.ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
			for (int i = z-1; i>=0; i--){
				TileProperties tile = GameObject.Find(x.ToString()+i.ToString()).GetComponent<TileProperties>();
				if(!tile.useBy)
					tile.canClick = true;
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
			for(int i = x+1; i < 8; i++){
				TileProperties tile = GameObject.Find(i.ToString()+z.ToString()).GetComponent<TileProperties>();
				if(!tile.useBy)
					tile.canClick = true;
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
			for(int i = x-1; i>=0; i--){
				TileProperties tile = GameObject.Find(i.ToString()+z.ToString()).GetComponent<TileProperties>();
				if(!tile.useBy)
					tile.canClick = true;
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
		}

		public void vmBishop(int x, int z){
			for(int i = 1; x+i<8 && z+i<8; i++){
				TileProperties tile = GameObject.Find((x+i).ToString()+(z+i).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy)
					tile.canClick = true;
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
			for(int i = 1; x-i >=0 && z-i >= 0; i++){
				TileProperties tile = GameObject.Find((x-i).ToString()+(z-i).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy)
					tile.canClick = true;
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
			for(int i = 1; x-i >= 0 && z+i < 8; i++){
				TileProperties tile = GameObject.Find((x-i).ToString()+(z+i).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy)
					tile.canClick = true;
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
			for(int i = 1; x+i < 8 && z-i >=0; i++){
				TileProperties tile = GameObject.Find((x+i).ToString()+(z-i).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy)
					tile.canClick = true;
				else{
					_piecesToEat(tile.useBy);
					break;
				}
			}
		}

		public void vmKing(int x, int z){
			if(x+1<8){
				TileProperties tile = GameObject.Find((x+1).ToString()+z.ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
			if(z+1<8){
				TileProperties tile = GameObject.Find(x.ToString()+(z+1).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
			if(x-1>=0){
				TileProperties tile = GameObject.Find((x-1).ToString()+z.ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
			if(z-1>=0){
				TileProperties tile = GameObject.Find(x.ToString()+(z-1).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
			if((x+1<8)&&(z+1<8)){
				TileProperties tile = GameObject.Find((x+1).ToString()+(z+1).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
			if((x-1>=0)&&(z-1>=0)){
				TileProperties tile = GameObject.Find((x-1).ToString()+(z-1).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
			if((x+1<8)&&(z-1>=0)){
				TileProperties tile = GameObject.Find((x+1).ToString()+(z-1).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
			if((x-1>=0)&&(z+1<8)){
				TileProperties tile = GameObject.Find((x-1).ToString()+(z+1).ToString()).GetComponent<TileProperties>();
				if(!tile.useBy){
					tile.canClick = true;
				}
				else{
					_piecesToEat(tile.useBy);
				}
			}
		}

		public void vmKnight(int x, int z){
			if(z+2<8){
				if(x-1>=0){
					TileProperties tile = GameObject.Find((x-1).ToString()+(z+2).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
				if(x+1<8){
					TileProperties tile = GameObject.Find((x+1).ToString()+(z+2).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
			}
			if(x+2<8){
				if(z+1<8){
					TileProperties tile = GameObject.Find((x+2).ToString()+(z+1).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
				if(z-1>=0){
					TileProperties tile = GameObject.Find((x+2).ToString()+(z-1).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
			}
			if(x-2>=0){
				if(z+1<8){
					TileProperties tile = GameObject.Find((x-2).ToString()+(z+1).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
				if(z-1>=0){
					TileProperties tile = GameObject.Find((x-2).ToString()+(z-1).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
			}
			if(z-2>=0){
				if(x+1<8){
					TileProperties tile = GameObject.Find((x+1).ToString()+(z-2).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
				if(x-1>=0){
					TileProperties tile = GameObject.Find((x-1).ToString()+(z-2).ToString()).GetComponent<TileProperties>();
					if(!tile.useBy){
						tile.canClick = true;
					}
					else{
						_piecesToEat(tile.useBy);
					}
				}
			}
		}
	}
}