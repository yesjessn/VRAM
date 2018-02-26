using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Subject;

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

	public static CSVWriter NewOutputFile(SubjectDataHolder subject, string name) {
		string folder = Application.persistentDataPath;
		string subjectFilePrefix = "";
		if (subject != null && subject.data.subjectId != null && subject.data.subjectId.Length > 0) {
			subjectFilePrefix = subject.data.subjectId + "_";
			folder = Path.Combine(folder, subject.data.subjectId);
			if(!Directory.Exists(folder)) {
				Directory.CreateDirectory(folder);
			}
		}
		return new CSVWriter(Path.Combine(folder, subjectFilePrefix + name + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss")+ ".csv"));
	}
}
