using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class CSVWriter {

	StreamWriter writer;

	public CSVWriter(string fileName) {
		writer = new StreamWriter (fileName);
		writer.AutoFlush = true;
	}

	public void WriteRow(string row) {
		writer.WriteLine(row);
	}

	public void Write(string rows) {
		writer.Write(rows);
	}

	public void Close () {
		writer.Close ();
	}

	public static CSVWriter NewOutputFile(string name) {
		string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
		desktop = Path.Combine(desktop, "OutputLogs");
		if(!Directory.Exists(desktop)) {
			Directory.CreateDirectory(desktop);
		}
		return new CSVWriter(Path.Combine(desktop, name + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss")+ ".csv"));
	}
}
