using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour {
	public SqliteDatabase database;

	private string _problem;
	private string _a;
	private string _b;
	private string _c;
	private string _d;
	private string _ans;
	private int score;

	// Use this for initialization
	void Start () {
		score = 0;
		database = new SqliteDatabase("testDB.db");
		getData();
	}

	void OnGUI(){
		guiQuiz();
	}
	
	void getData(){
		var data = database.ExecuteQuery("select * from Question where status = 0");
		int indx = Random.Range(0, data.Rows.Count);
		_problem = data.Rows[indx]["prob"].ToString();
		_a = data.Rows[indx]["a"].ToString();
		_b = data.Rows[indx]["b"].ToString();
		_c = data.Rows[indx]["c"].ToString();
		_d = data.Rows[indx]["d"].ToString();
		_ans = data.Rows[indx]["ans"].ToString();
	}

	void quizCorrect(){
		database.ExecuteNonQuery("update Question set status = 1 where prob='"+_problem+"'");
		score++;
		getData();
	}

	void quizWrong(){
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUI.skin.button.fontSize = (int)Screen.width / 10;
		if(GUI.Button(new Rect(Screen.width*.05f, Screen.height*.05f, Screen.width*.9f, Screen.height*.9f),"Game Over!\nYour Score: "+score)){
			database.ExecuteNonQuery("update Question set status=0");
			Application.LoadLevel(0);
		}
	}

	void guiQuiz(){
		GUI.skin.button.fontSize = (int)Screen.width / 40;
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUI.skin.button.wordWrap = true;
		GUI.skin.box.fontSize = (int)Screen.width / 30;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		GUI.skin.box.wordWrap = true;
		
		GUI.Box(new Rect(Screen.width*.05f, Screen.height*.05f, Screen.width*.9f, Screen.height*.9f),"");
		GUI.Box(new Rect(Screen.width*.10f, Screen.height*.10f, Screen.width*.80f, Screen.height*.50f),_problem);
		
		if (GUI.Button(new Rect(Screen.width*.10f, Screen.height*.65f, Screen.width*.35f, Screen.height*.10f),_a)){
			if(_a==_ans)
				quizCorrect();
			else
				quizWrong();
		}
		if (GUI.Button(new Rect(Screen.width*.55f, Screen.height*.65f, Screen.width*.35f, Screen.height*.10f),_b)){
			if(_b==_ans)
				quizCorrect();
			else
				quizWrong();
		}
		if (GUI.Button(new Rect(Screen.width*.10f, Screen.height*.80f, Screen.width*.35f, Screen.height*.10f),_c)){
			if(_c==_ans)
				quizCorrect();
			else
				quizWrong();
		}
		if (GUI.Button(new Rect(Screen.width*.55f, Screen.height*.80f, Screen.width*.35f, Screen.height*.10f),_d)){
			if(_d==_ans)
				quizCorrect();
			else
				quizWrong();
		}
	}
}
