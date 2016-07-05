using System;

namespace Smartlogic.Semaphore.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class ClassificationItem
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly float _score;
        /// <summary>
        /// 
        /// </summary>
        private readonly string _value;
        /// <summary>
        /// 
        /// </summary>
        private readonly string _id;
        /// <summary>
        /// 
        /// </summary>
        private readonly string _classname;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationItem"/> class.
        /// </summary>
        /// <param name="classname">The classname.</param>
        /// <param name="value">The value.</param>
        /// <param name="score">The score.</param>
        /// <remarks></remarks>
        public ClassificationItem(string classname,string value, float score)
        {
            _classname = classname;
            _value = value;
            _id = Value;
            _score = score;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationItem"/> class.
        /// </summary>
        /// <param name="classname">The classname.</param>
        /// <param name="value">The value.</param>
        /// <param name="score">The score.</param>
        /// <param name="id">The id.</param>
        /// <remarks></remarks>
        public ClassificationItem(string classname,string value, float score, string id)
        {
            _classname = classname;
            _value = value;
            _id=id;
            _score = score;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <remarks></remarks>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <remarks></remarks>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <remarks></remarks>
        public string ClassName
        {
            get { return _classname; }
        }

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <remarks></remarks>
        public float Score
        {
            get { return _score; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var ca = obj as ClassificationItem;
            if (ca != null) return Equals(ca);
            return base.Equals(obj);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Equals(ClassificationItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._score.Equals(_score) && Equals(other._value, _value) && Equals(other._id, _id) && Equals(other._classname, _classname);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = _score.GetHashCode();
                result = (result*397) ^ (_value != null ? _value.GetHashCode() : 0);
                result = (result*397) ^ (_id != null ? _id.GetHashCode() : 0);
                result = (result*397) ^ (_classname != null ? _classname.GetHashCode() : 0);
                return result;
            }
        }
    }
}