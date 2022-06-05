using Common;
using Common.Xml;

using System;
using System.Collections.Generic;
using System.Reflection;

using Unity.Mathematics;

using UnityEngine;

namespace Game {
    /// <summary>
    /// A utility class that loads object properties from an XML
    /// </summary>
    public class InstanceLoader {
        private readonly Type type;
        private readonly PropertyInfo[] properties;

        private delegate void PropertyLoader(SimpleXmlNode node, PropertyInfo property, object instance);
        private readonly Dictionary<Type, PropertyLoader> attributeLoaderMap = new Dictionary<Type, PropertyLoader>();
        private readonly Dictionary<Type, PropertyLoader> elementLoaderMap = new Dictionary<Type, PropertyLoader>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public InstanceLoader(Type type) {
            this.type = type;
            this.properties = this.type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Populate loader maps
            this.attributeLoaderMap[typeof(string)] = LoadString;
            this.attributeLoaderMap[typeof(int)] = LoadInt;
            this.attributeLoaderMap[typeof(float)] = LoadFloat;
            this.attributeLoaderMap[typeof(bool)] = LoadBool;
            this.attributeLoaderMap[typeof(ulong)] = LoadULong;
            this.attributeLoaderMap[typeof(long)] = LoadULong;

            this.elementLoaderMap[typeof(Vector3)] = LoadVector3;
            this.elementLoaderMap[typeof(Color)] = LoadColor;
            this.elementLoaderMap[typeof(int[])] = LoadIntArray;

            this.elementLoaderMap[typeof(ContributionType)] = LoadContributionType;
        }

        public void Load(SimpleXmlNode node, object instance) {
            foreach (PropertyInfo property in this.properties) {
                if (!TypeUtils.IsVariableProperty(property)) {
                    continue;
                }

                // Must have the Persist attribute
                object[] attributes = property.GetCustomAttributes(typeof(Persist), false);
                if (attributes.Length == 0) {
                    // No Persist attribute
                    continue;
                }

                // Must have a loader
                bool loaded = false;
                if (this.attributeLoaderMap.TryGetValue(property.PropertyType, out PropertyLoader propLoader)) {
                    // Invokes the property loader
                    propLoader(node, property, instance);
                    loaded = true;
                }

                if (this.elementLoaderMap.TryGetValue(property.PropertyType, out propLoader)) {
                    propLoader(node, property, instance);
                    loaded = true;
                }

                Assertion.IsTrue(loaded, property.Name + " was not loaded"); // Might forget the loader
            }
        }

        private static void SetDefault(PropertyInfo property, object instance) {
            // Get only the first one
            object[] attributes = property.GetCustomAttributes(typeof(Persist), false);
            Assertion.IsTrue(attributes.Length > 0); // Must have a persist attribute

            object defaultValue = ((Persist) attributes[0]).DefaultValue;
            property.GetSetMethod().Invoke(instance, new[] {defaultValue});
        }

        private static void LoadString(SimpleXmlNode node, PropertyInfo property, object instance) {
            if (!node.HasAttribute(property.Name)) {
                SetDefault(property, instance);
                return;
            }

            string value = node.GetAttribute(property.Name);
            property.GetSetMethod().Invoke(instance, new object[] {value});
        }

        private static void LoadInt(SimpleXmlNode node, PropertyInfo property, object instance) {
            if (!node.HasAttribute(property.Name)) {
                SetDefault(property, instance);
                return;
            }

            int value = node.GetAttributeAsInt(property.Name);
            property.GetSetMethod().Invoke(instance, new object[] {value});
        }

        private static void LoadULong(SimpleXmlNode node, PropertyInfo property, object instance) {
            if (!node.HasAttribute(property.Name)) {
                SetDefault(property, instance);
                return;
            }

            ulong value = ulong.Parse(node.GetAttribute(property.Name));
            property.GetSetMethod().Invoke(instance, new object[] {value});
        }

        private static void LoadFloat(SimpleXmlNode node, PropertyInfo property, object instance) {
            if (!node.HasAttribute(property.Name)) {
                SetDefault(property, instance);
                return;
            }

            float value = node.GetAttributeAsFloat(property.Name);
            property.GetSetMethod().Invoke(instance, new object[] {value});
        }

        private static void LoadBool(SimpleXmlNode node, PropertyInfo property, object instance) {
            if (!node.HasAttribute(property.Name)) {
                SetDefault(property, instance);
                return;
            }

            bool value = node.GetAttributeAsBool(property.Name);
            property.GetSetMethod().Invoke(instance, new object[] {value});
        }

        /// <summary>
        /// Common method for loading an int2 from the specified node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static int2 LoadInt2(SimpleXmlNode node) {
            int2 position = new int2 {
                x = node.GetAttributeAsInt("x"), y = node.GetAttributeAsInt("y")
            };

            return position;
        }

        private static void LoadVector3(SimpleXmlNode node, PropertyInfo property, object instance) {
            // Note that this requires an element
            SimpleXmlNode propertyNode = node.FindFirstNodeInChildren(property.Name);
            if (propertyNode == null) {
                // No element for the property was found
                SetDefault(property, instance);
                return;
            }

            Vector3 vec = new Vector3();
            vec.x = propertyNode.GetAttributeAsFloat("x");
            vec.y = propertyNode.GetAttributeAsFloat("y");
            vec.z = propertyNode.GetAttributeAsFloat("z");

            property.GetSetMethod().Invoke(instance, new object[] {vec});
        }

        private static void LoadColor(SimpleXmlNode node, PropertyInfo property, object instance) {
            // Note that this requires an element
            SimpleXmlNode propertyNode = node.FindFirstNodeInChildren(property.Name);
            if (propertyNode == null) {
                // No element for the property was found
                SetDefault(property, instance);
                return;
            }

            Color color = new Color();
            color.r = ResolveColorValue(propertyNode.GetAttributeAsFloat("r"));
            color.g = ResolveColorValue(propertyNode.GetAttributeAsFloat("g"));
            color.b = ResolveColorValue(propertyNode.GetAttributeAsFloat("b"));
            color.a = ResolveColorValue(propertyNode.GetAttributeAsFloat("a"));

            property.GetSetMethod().Invoke(instance, new object[] {color});
        }

        private static float ResolveColorValue(float loadedColor) {
            if (loadedColor > 1.0f) {
                // Save file might already be broken
                // We return a random value instead
                return UnityEngine.Random.Range(0.1f, 1.0f);
            }

            return loadedColor;
        }

        private static readonly SimpleList<int> ELEMENT_VALUES = new SimpleList<int>();

        private static void LoadIntArray(SimpleXmlNode node, PropertyInfo property, object instance) {
            // Note that this requires an element
            SimpleXmlNode propertyNode = node.FindFirstNodeInChildren(property.Name);
            if (propertyNode == null) {
                // No element for the property was found
                SetDefault(property, instance);
                return;
            }

            // Collect values first
            ELEMENT_VALUES.Clear();
            for(int i = 0; i < propertyNode.Children.Count; ++i) {
                SimpleXmlNode child = propertyNode.Children[i];
                if (InstanceWriter.ELEMENT.EqualsFast(child.tagName)) {
                    // It's an array value
                    ELEMENT_VALUES.Add(child.GetAttributeAsInt(InstanceWriter.VALUE));
                }
            }

            // Set the int array value
            property.GetSetMethod().Invoke(instance, new object[] { ELEMENT_VALUES.ToArray() });

            ELEMENT_VALUES.Clear();
        }

        private static void LoadContributionType(SimpleXmlNode node, PropertyInfo property, object instance) {
            if (!node.HasAttribute(property.Name)) {
                SetDefault(property, instance);
                return;
            }

            string contributionTypeId = node.GetAttribute(property.Name);
            ContributionType value = ContributionType.ConvertFromId(contributionTypeId);
            property.GetSetMethod().Invoke(instance, new object[] {value});
        }
    }
}