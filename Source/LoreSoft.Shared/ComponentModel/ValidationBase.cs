using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using LoreSoft.Shared.Reflection;

namespace LoreSoft.Shared.ComponentModel
{
    /// <summary>
    /// An enum to determine when to validate a property.
    /// </summary>
    public enum ValidatePropertyMode
    {
        /// <summary>Always validate properties.</summary>
        Always,
        /// <summary>Only validate properties that current have an error.</summary>
        HasError,
        /// <summary>Never validate properties.</summary>
        Never
    }

    /// <summary>
    /// A base class that implements common property validation methods.
    /// </summary>
    [DataContract]
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class ValidationBase
        : NotificationBase
#if SILVERLIGHT
        , INotifyDataErrorInfo
#else
        , IDataErrorInfo
#endif
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBase"/> class.
        /// </summary>
        protected ValidationBase()
        { }

#if !SILVERLIGHT
        [NonSerialized]
#endif
        private ErrorsContainer _errorsContainer;
        /// <summary>
        /// Gets the errors container which holds the list of validation errors.
        /// </summary>
        protected ErrorsContainer ErrorsContainer
        {
            get
            {
                if (_errorsContainer == null)
                {
#if SILVERLIGHT
                    _errorsContainer = new ErrorsContainer(RaiseErrorsChanged);
#else
                    _errorsContainer = new ErrorsContainer();
#endif
                }

                return _errorsContainer;
            }
        }

        /// <summary>
        /// Gets or sets the validate property mode. This property is used to control when the ValidateProperty can be called.
        /// </summary>
        /// <value>
        /// The validate property mode.
        /// </value>
        /// <seealso cref="ValidateProperty"/>
        /// <seealso cref="ShouldValidateProperty"/>
        protected ValidatePropertyMode ValidatePropertyMode { get; set; }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The expression to get the property name.</param>
        /// <param name="value">The value of the property.</param>
        protected void ValidateProperty<TProperty>(Expression<Func<TProperty>> propertyExpression, object value)
        {
            if (ValidatePropertyMode == ValidatePropertyMode.Never)
                return;

            var propertyName = ReflectionHelper.ExtractPropertyName(propertyExpression);
            ValidateProperty(propertyName, value);
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="propertyName">Name of the property to validate.</param>
        /// <param name="value">The value of the property.</param>
        protected virtual void ValidateProperty(string propertyName, object value)
        {
            if (!ShouldValidateProperty(propertyName))
                return;

            var vc = new ValidationContext(this, null, null) { MemberName = propertyName, };
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateProperty(value, vc, results);

            if (isValid)
                ErrorsContainer.ClearErrors(propertyName);
            else
                ErrorsContainer.SetErrors(propertyName, results.Select(s => s.ErrorMessage));

            OnValidationComplete();
        }

        /// <summary>
        /// Determine whether the property should be validated from the ValidatePropertyMode setting.
        /// </summary>
        /// <param name="propertyName">Name of the property to validate.</param>
        /// <returns><c>true</c> if the property should be validated; otherwise <c>false</c>.</returns>
        protected bool ShouldValidateProperty(string propertyName)
        {
            if (ValidatePropertyMode == ValidatePropertyMode.Never)
                return false;

            if (ValidatePropertyMode == ValidatePropertyMode.HasError
                && ErrorsContainer.GetErrors(propertyName).Any() == false)
                return false;

            return true;
        }

        /// <summary>
        /// Called when validation completes.
        /// </summary>
        protected virtual void OnValidationComplete()
        {

        }

        /// <summary>
        /// Validates this instance's properties.
        /// </summary>
        /// <returns><c>true</c> if the instance is valid.</returns>
        public bool Validate()
        {
            return Validate(true);
        }

        /// <summary>
        /// Validates this instance's properties.
        /// </summary>
        /// <param name="updateDisplay">if set to <c>true</c> the display is notified.</param>
        /// <returns><c>true</c> if the instance is valid.</returns>
        public virtual bool Validate(bool updateDisplay)
        {
            var vc = new ValidationContext(this, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(this, vc, results, true);

            if (!updateDisplay)
                return isValid;

            if (isValid)
                ErrorsContainer.ClearErrors();
            else
                ErrorsContainer.SetErrors(results);

            OnValidationComplete();
            return isValid;
        }

#if SILVERLIGHT
    /// <summary>
    /// Occurs when the validation errors have changed for a property or for the entire object.
    /// </summary>
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    /// <summary>
    /// Raises the errors changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void RaiseErrorsChanged(string propertyName)
    {
      var handler = ErrorsChanged;
      if (handler == null)
        return;

      var e = new DataErrorsChangedEventArgs(propertyName);
      handler(this, e);
    }

    /// <summary>
    /// Raises the errors changed.
    /// </summary>
    /// <param name="propertyNames">The property names.</param>
    protected void RaiseErrorsChanged(params string[] propertyNames)
    {
      if (propertyNames == null)
        throw new ArgumentNullException("propertyNames");

      foreach (var name in propertyNames)
        RaiseErrorsChanged(name);
    }

    /// <summary>
    /// Gets the validation errors for a specified property or for the entire object.
    /// </summary>
    /// <param name="propertyName">The name of the property to retrieve validation errors for, or null or <see cref="F:System.String.Empty"/> to retrieve errors for the entire object.</param>
    /// <returns>
    /// The validation errors for the property or object.
    /// </returns>
    IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
    {
      return ErrorsContainer.GetErrors(propertyName);
    }

    /// <summary>
    /// Gets a value that indicates whether the object has validation errors.
    /// </summary>
    /// <returns>true if the object currently has validation errors; otherwise, false.</returns>
    bool INotifyDataErrorInfo.HasErrors
    {
      get { return ErrorsContainer.HasErrors; }
    }
#else
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        string IDataErrorInfo.this[string columnName]
        {
            get { return ErrorsContainer.GetErrors(columnName).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        string IDataErrorInfo.Error
        {
            get { return ErrorsContainer.ToString(); }
        }
#endif
    }
}
