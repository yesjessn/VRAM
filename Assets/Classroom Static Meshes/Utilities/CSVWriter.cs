using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class CSVWriter {

	StreamWriter writer;

	public CSVWriter(string fileName) {
		writer = new StreamWriter (fileName);
	}

	public void WriteRow(string row) {
		writer.WriteLine (row);
	}

	public void Close () {
		writer.Close ();
	}
}
