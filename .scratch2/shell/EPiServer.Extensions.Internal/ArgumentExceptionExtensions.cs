using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EPiServer.Extensions.Internal;

internal static class ArgumentExceptionExtensions
{
	extension(ArgumentException)
	{
		/// <summary>
		///       Throws an <see cref="T:System.ArgumentException" /> if the property of an object is null.
		///       </summary>
		public static void ThrowIfPropertyIsNull([NotNull] object property, string? message = null, string? paramName = null, [CallerArgumentExpression("property")] string? propertyName = null)
		{
			if (property == null)
			{
				ThrowPropertyArgumentException("Value property cannot be null.", message, paramName, propertyName);
			}
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentException" /> if the string property of an object is null or empty.
		///       </summary>
		public static void ThrowIfPropertyIsNullOrEmpty([NotNull] string property, string? message = null, string? paramName = null, [CallerArgumentExpression("property")] string? propertyName = null)
		{
			if (string.IsNullOrEmpty(property))
			{
				ThrowPropertyArgumentException("Value property cannot be null or empty.", message, paramName, propertyName);
			}
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentException" /> if the string property of an object is null or empty.
		///       </summary>
		public static void ThrowIfPropertyIsNullOrWhitespace([NotNull] string property, string? message = null, string? paramName = null, [CallerArgumentExpression("property")] string? propertyName = null)
		{
			if (string.IsNullOrWhiteSpace(property))
			{
				ThrowPropertyArgumentException("Value property cannot be null or empty.", message, paramName, propertyName);
			}
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentException" /> if the predicate is true for property.
		///       </summary>
		public static void ThrowIfProperty<T>(T property, Func<T, bool> predicate, string? message = null, string? paramName = null, [CallerArgumentExpression("property")] string? propertyName = null)
		{
			if (predicate(property))
			{
				ThrowPropertyArgumentException("Value property not valid.", message, paramName, propertyName);
			}
		}

		[DoesNotReturn]
		private static void ThrowPropertyArgumentException(string defaultMessage, string? message, string? paramName, string? propertyName)
		{
			if (message == null && propertyName != null)
			{
				string[] array = propertyName.Split('.', 2);
				if (array != null && array.Length == 2)
				{
					if (paramName == null)
					{
						paramName = array[0];
					}
					propertyName = array[1];
				}
				message = defaultMessage + " (Property: '" + propertyName + "')";
			}
			throw new ArgumentException(message, paramName);
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentException" /> if the predicate is true.
		///       </summary>
		public static void ThrowIf<T>(T argument, Func<T, bool> predicate, string message, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (predicate(argument))
			{
				throw new ArgumentException(message, paramName);
			}
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentException" /> if the <see cref="T:System.Guid" /> is empty.
		///       </summary>
		public static void ThrowIfEmpty(Guid argument, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (argument == Guid.Empty)
			{
				throw new ArgumentException("Value cannot be an empty GUID.", paramName);
			}
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentNullException" /> if the <see cref="T:System.Globalization.CultureInfo" /> is null or 
		///       an <see cref="T:System.ArgumentException" /> if it's invariant.
		///       </summary>
		public static void ThrowIfNullOrInvariant(CultureInfo argument, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(paramName);
			}
			if (argument.Equals(CultureInfo.InvariantCulture))
			{
				throw new ArgumentException("Value cannot be an invariant culture.", paramName);
			}
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentNullException" /> if the <see cref="T:System.Collections.Generic.IEnumerable`1" /> is null or 
		///       an <see cref="T:System.ArgumentException" /> if it's empty.
		///       </summary>
		public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> argument, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(paramName);
			}
			if (!argument.Any())
			{
				throw new ArgumentException("Value cannot be an empty list.", paramName);
			}
		}

		/// <summary>
		///       Throws an <see cref="T:System.ArgumentNullException" /> if the <see cref="T:System.Collections.Generic.IEnumerable`1" /> is null or 
		///       an <see cref="T:System.ArgumentException" /> if it contain any null values.
		///       </summary>
		public static void ThrowIfNullOrContainsNull<T>(IEnumerable<T> argument, [CallerArgumentExpression("argument")] string? paramName = null) where T : class
		{
			if (argument == null)
			{
				throw new ArgumentNullException(paramName);
			}
			if (argument.Any((T x) => x == null))
			{
				throw new ArgumentException("Value cannot contain items that are null.", paramName);
			}
		}
	}
}
