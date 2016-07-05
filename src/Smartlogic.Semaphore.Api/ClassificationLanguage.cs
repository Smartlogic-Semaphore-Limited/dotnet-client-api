using System;

namespace Smartlogic.Semaphore.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class ClassificationLanguage
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks></remarks>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks></remarks>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has rules defined.
        /// </summary>
        /// <value><c>true</c> if this instance has rules defined; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool HasRulesDefined { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        /// <remarks></remarks>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        public string Name { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            
            return base.Equals(obj);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Equals(ClassificationLanguage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Id, Id) && other.HasRulesDefined.Equals(HasRulesDefined) && other.IsDefault.Equals(IsDefault) && Equals(other.DisplayName, DisplayName) && Equals(other.Name, Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Type != null ? Type.GetHashCode() : 0);
                result = (result*397) ^ (Id != null ? Id.GetHashCode() : 0);
                result = (result*397) ^ HasRulesDefined.GetHashCode();
                result = (result*397) ^ IsDefault.GetHashCode();
                result = (result*397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                return result;
            }
        }
    }
}