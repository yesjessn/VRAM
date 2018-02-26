using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Subject {
    public class SubjectDataHolder : MonoBehaviour {
        public SubjectData data;

        public void SetSubjectId(string subjectId) {
            data.subjectId = subjectId;
            LoadSubjectData();
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
                var serializer = new CsvFormatter<SessionData>(',', true);
                FileStream stream = File.Open(sessionsFile, FileMode.Open);
                data.sessions = (List<SessionData>) serializer.Deserialize(stream);
                stream.Close();
            }
        }

        public void SaveSubjectData() {
            string folder = Application.persistentDataPath;
            folder = Path.Combine(folder, data.subjectId);
            if (!Directory.Exists(folder)) {
				Directory.CreateDirectory(folder);
            }

            string sessionsFile = Path.Combine(folder, "sessions.csv");
            {
                var serializer = new CsvFormatter<SessionData>(',', true);
                FileStream stream = File.Create(sessionsFile);
                serializer.Serialize(stream, data.sessions);
                stream.Close();
            }
        }

        void OnDisable() {
            SaveSubjectData();
        }
    }
}
