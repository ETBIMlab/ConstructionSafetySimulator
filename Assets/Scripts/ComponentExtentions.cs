using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Contains classes of helper functions that are not build into the unity engine. See "StaticFunctions.cs" for examples.
/// </summary>
namespace ETBIM.StaticFunctions
{
    /// <summary>
    /// Static Extentions for unitys Components.
    /// </summary>
    public static class ComponentExtentions
    {
        /// <summary>
        /// Copys component properties and fields between two components.
        /// </summary>
        /// <typeparam name="T">Component type. Type of comp and other should equal</typeparam>
        /// <param name="dest">[Destination] Extended component where values are copyed to and overrided.</param>
        /// <param name="source">[source] input component where values are copyed from.</param>
        /// <returns></returns>
        public static bool CopyFrom<T>(this Component dest, T source) where T : Component
        {
        // Check types
            Type type = dest.GetType();
            if (type != source.GetType()) return false; // type mis-match

        // Copy Properties
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    // try-catch is tested for each individual property.
                    try
                    {
                        pinfo.SetValue(dest, pinfo.GetValue(source, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }

        // Copy Fields
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(dest, finfo.GetValue(source));
            }
            return true;
        }
    }
}
