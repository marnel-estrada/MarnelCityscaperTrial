using System;

namespace Game {
    /// <summary>
    /// An attribute class to mark a property as write candidate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Persist : Attribute {
        private object defaultValue;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Persist() {
        }
        
        /// <summary>
        /// The default value if no value exists
        /// </summary>
        /// <param name="defaultValue"></param>
        public Persist(object defaultValue) {
            this.defaultValue = defaultValue;
        }

        public object DefaultValue {
            get {
                return this.defaultValue;
            }
        }
    }
}