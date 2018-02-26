using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subject {
    [Serializable]
    public class SubjectData {
        public string subjectId;
        
        public List<SessionData> sessions;
    }
}
