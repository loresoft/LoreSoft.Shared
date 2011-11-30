using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LoreSoft.Shared.Collections;
using LoreSoft.Shared.Reflection;

namespace LoreSoft.Shared.ComponentModel
{
    /// <summary>
    /// Manages validation errors for an object, notifying when the error state changes.
    /// </summary>
    public class ErrorsContainer
    {
        private readonly Action<string> _raiseErrorsChanged;
        private readonly MultiDictionary<string, string> _validationResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsContainer"/> class.
        /// </summary>
        public ErrorsContainer()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsContainer"/> class.
        /// </summary>
        /// <param name="raiseErrorsChanged">The action that invoked if when errors are added for an object./>
        /// event.</param>
        public ErrorsContainer(Action<string> raiseErrorsChanged)
        {
            _raiseErrorsChanged = raiseErrorsChanged;
            _validationResults = new MultiDictionary<string, string>();
        }

        /// <summary>
        /// Gets a value indicating whether the object has validation errors. 
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return _validationResults.Count != 0;
            }
        }

        public ILookup<string, string> Results
        {
            get { return _validationResults; }
        }

        /// <summary>
        /// Gets the validation errors for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The validation errors for the property.</returns>
        public IEnumerable<string> GetErrors(string propertyName)
        {
            var name = propertyName ?? string.Empty;
            HashSet<string> errors;
            _validationResults.TryGetValue(name, out errors);
            return errors ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Clears the errors for the property indicated by the property expression.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The expression indicating a property.</param>
        /// <example>
        ///     container.ClearErrors(()=>SomeProperty);
        /// </example>
        public void ClearErrors<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var propertyName = ReflectionHelper.ExtractPropertyName(propertyExpression);
            ClearErrors(propertyName);
        }

        /// <summary>
        /// Clears the errors for a property.
        /// </summary>
        /// <param name="propertyName">The name of th property for which to clear errors.</param>
        /// <example>
        ///     container.ClearErrors("SomeProperty");
        /// </example>
        public void ClearErrors(string propertyName)
        {
            var name = propertyName ?? string.Empty;

            if (!_validationResults.Remove(name))
                return;

            if (_raiseErrorsChanged != null)
                _raiseErrorsChanged(name);
        }

        /// <summary>
        /// Clears all errors
        /// </summary>
        public void ClearErrors()
        {
            var propertyNames = _validationResults.Keys.ToArray();
            foreach (var propertyName in propertyNames)
                ClearErrors(propertyName);

            _validationResults.Clear();
        }

        /// <summary>
        /// Sets the validation error for the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The property type for which to set errors.</typeparam>
        /// <param name="propertyExpression">The <see cref="Expression"/> indicating the property.</param>
        /// <param name="errorMessage">The error message to set for the property.</param>
        public void SetError<TProperty>(Expression<Func<TProperty>> propertyExpression, string errorMessage)
        {
            var propertyName = ReflectionHelper.ExtractPropertyName(propertyExpression);
            SetError(propertyName, errorMessage);
        }

        /// <summary>
        /// Sets the validation error for the specified property.
        /// </summary>
        /// <remarks>
        /// If a change is detected then the errors changed event is raised.
        /// </remarks>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="errorMessage">The error message to set for the property.</param>
        public void SetError(string propertyName, string errorMessage)
        {
            var name = propertyName ?? string.Empty;

            _validationResults.Remove(name);
            _validationResults.Add(name, errorMessage);

            if (_raiseErrorsChanged != null)
                _raiseErrorsChanged(name);
        }

        /// <summary>
        /// Sets the validation errors for the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The property type for which to set errors.</typeparam>
        /// <param name="propertyExpression">The <see cref="Expression"/> indicating the property.</param>
        /// <param name="propertyErrors">The list of errors to set for the property.</param>
        public void SetErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, IEnumerable<string> propertyErrors)
        {
            var propertyName = ReflectionHelper.ExtractPropertyName(propertyExpression);
            SetErrors(propertyName, propertyErrors);
        }

        /// <summary>
        /// Sets the validation errors for the specified property.
        /// </summary>
        /// <remarks>
        /// If a change is detected then the errors changed event is raised.
        /// </remarks>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="results">The new validation errors.</param>
        public void SetErrors(string propertyName, IEnumerable<string> results)
        {
            var name = propertyName ?? string.Empty;

            _validationResults.Remove(name);
            _validationResults.AddRange(name, results);

            if (_raiseErrorsChanged != null)
                _raiseErrorsChanged(name);
        }

        public void SetErrors(IEnumerable<ValidationResult> validationErrors)
        {
            // need to clear errors first as new results might not have old name
            ClearErrors();

            foreach (ValidationResult validationResult in validationErrors)
            {
                var memberNames = validationResult.MemberNames.ToList();

                if (memberNames.Count > 0)
                {
                    foreach (string memberName in memberNames)
                    {
                        _validationResults.Add(memberName, validationResult.ErrorMessage);

                        if (_raiseErrorsChanged != null)
                            _raiseErrorsChanged(memberName);
                    }
                }
                else
                {
                    _validationResults.Add(string.Empty, validationResult.ErrorMessage);
                    if (_raiseErrorsChanged != null)
                        _raiseErrorsChanged(string.Empty);
                }
            }
        }

        public IEnumerable<string> AllErrors()
        {
            return _validationResults
              .Values
              .SelectMany(errors => errors);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            foreach (string error in AllErrors())
                buffer.AppendLine(error);

            return buffer.ToString();
        }
    }
}
