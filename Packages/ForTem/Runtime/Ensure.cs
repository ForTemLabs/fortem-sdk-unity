using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable

namespace ForTemSdk
{
    internal static class Ensure
    {
        public static T ArgumentNotNull<T>([NotNull] T? obj, [CallerArgumentExpression("obj")] string paramName = "")
        {
            return obj == null
                ? throw new ArgumentNullException(paramName)
                : obj;
        }

        public static string ArgumentNotNullOrEmpty([NotNull] string? str, [CallerArgumentExpression("str")] string paramName = "")
        {
            Ensure.ArgumentNotNull(str, paramName);

            return str == ""
                ? throw new ArgumentException("Cannot be empty.", paramName)
                : str;
        }
    }
}
