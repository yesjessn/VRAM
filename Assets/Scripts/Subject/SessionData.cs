using System;

namespace Subject {
    [Serializable]
    public class SessionData {
        public int sessionNumber;
        public DateTime completionDate;
        public float salience;

        public SessionData(DateTime completionDate, float salience) {
            this.completionDate = completionDate;
            this.salience = salience;
        }
    }
}