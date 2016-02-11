using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

public class DBManager
{
	public DBHandlerAI DBHandlerAI;

	private IDbConnection _mainDBConn;

	public void Initialize()
	{
		DBHandlerAI = new DBHandlerAI();

		//open main db
		string conn = "URI=file:" + Application.dataPath + "/GameData/Database/Main.s3db"; //Path to database.
		Debug.Log("Main Database Path is : "+conn);
		_mainDBConn = (IDbConnection)new SqliteConnection(conn);
		_mainDBConn.Open(); //Open connection to the database.

		/*
		while (reader.Read ()) 
		{
			int value = reader.GetInt32(0);
			Debug.Log("value= " + value);
		}
		*/

		//following is a test
		DBHandlerAI.GetCharacterActionSet(0);
	}

	public IDataReader RunQuery(string query)
	{
		IDbCommand dbcmd = _mainDBConn.CreateCommand();
		dbcmd.CommandText = query;
		IDataReader reader = dbcmd.ExecuteReader();
		dbcmd.Dispose();
		dbcmd = null;

		return reader;
	}

}
