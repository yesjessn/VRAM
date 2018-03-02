using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Subject {
    public class SubjectDataHolder : MonoBehaviour {
        public static SubjectDataHolder instance;

        public SubjectData data = new SubjectData();

        void Awake() {
            if(instance != null) {
                Destroy(this.gameObject);
            } else {
                instance = this;
            }
        }
        
        public void SetSubjectId(string subjectId) {
            data.subjectId = subjectId;
        }

        public void AppendSession(SessionData session) {
            session.sessionNumber = data.sessions.Count;
            data.sessions.Add(session);
        }

        public void LoadSubjectData() {
            string folder = Application.persistentDataPath;
            folder = Path.Combine(folder, data.subjectId);
            if (!Directory.Exists(folder)) {
                return;
            }

            string sessionsFile = Path.Combine(folder, "sessions.csv");
            if (File.Exists(sessionsFile)) {
                var serializer = new CsvFormatter<SessionData>(',');
                FileStream stream = File.Open(sessionsFile, FileMode.Open);
                data.sessions = (List<SessionData>) serializer.Deserialize(stream);
                stream.Close();
            }
            Debug.Log(String.Format("Loaded {0} sessions for '{1}'", data.sessions.Count, data.subjectId), this.gameObject);
        }

        public void SaveSubjectData() {
            string folder = Application.persistentDataPath;
            folder = Path.Combine(folder, data.subjectId);
            if (!Directory.Exists(folder)) {
				Directory.CreateDirectory(folder);
            }

            string sessionsFile = Path.Combine(folder, "sessions.csv");
            {
                var serializer = new CsvFormatter<SessionData>(',');
                FileStream stream = File.Create(sessionsFile);
                serializer.Serialize(stream, data.sessions);
                stream.Close();
            }
        }
    }
}
