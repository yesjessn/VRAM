using System;

namespace Subject {
    [Serializable]
    public class SessionData {
        public int sessionNumber;
        DateTime completionDate;

        public SessionData(DateTime completionDate) {
            this.completionDate = completionDate;
        }
    }
}