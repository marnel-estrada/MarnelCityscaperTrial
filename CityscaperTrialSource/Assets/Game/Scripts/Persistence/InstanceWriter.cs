using Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

using UnityEngine;

namespace Game {
    /// <summary>
    /// A utility class that writes an instance to XML using reflection
    /// </summary>
    public class InstanceWriter {
        private readonly Type type;
        private readonly PropertyInfo[] properties;

        private delegate void PropertyWriter(XmlWriter writer, PropertyInfo property, object instance);

        private readonly Dictionary<Type, PropertyWriter> attributeWriterMap = new Dictionary<Type, PropertyWriter>();
        private readonly Dictionary<Type, PropertyWriter> elementWriterMap = new Dictionary<Type, PropertyWriter>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public InstanceWriter(Type type) {
            this.type = type;
            this.properties = this.type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Populate writerMap
            this.attributeWriterMap[typeof(string)] = WriteAsAttribute;
            this.attributeWriterMap[typeof(int)] = WriteInt;

            // We use a different writer of float due to CultureInfo.InvariantCulture.NumberFormat
            this.attributeWriterMap[typeof(float)] = WriteFloat;
            this.attributeWriterMap[typeof(bool)] = WriteAsAttribute;
            this.attributeWriterMap[typeof(ulong)] = WriteAsAttribute;
            this.attributeWriterMap[typeof(long)] = WriteAsAttribute;
            this.attributeWriterMap[typeof(DateTime)] = WriteDateTime;
            this.attributeWriterMap[typeof(ContributionType)] = WriteContributionType;
            this.attributeWriterMap[typeof(Status)] = WriteStatus;

            this.elementWriterMap[typeof(Vector3)] = WriteVector3;
            this.elementWriterMap[typeof(Color)] = WriteColor;
            this.elementWriterMap[typeof(int[])] = WriteIntArray;
        }

        private readonly SimpleList<PropertyInfo> requiresElementWriterProperties = new SimpleList<PropertyInfo>();
        private readonly SimpleList<PropertyInfo> serializableProperties = new SimpleList<PropertyInfo>();

        /// <summary>
        /// Writes the specified instance of the type
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="instance"></param>
        public void Write(XmlWriter writer, object instance) {
            Start(writer);
            WriteProperties(writer, instance);
            End(writer);
        }

        /// <summary>
        /// Starts the element for the instance
        /// We designed it this way so that client code can insert special elements in between
        /// </summary>
        /// <param name="writer"></param>
        public void Start(XmlWriter writer) {
            this.requiresElementWriterProperties.Clear();
            this.serializableProperties.Clear();
            writer.WriteStartElement(this.type.Name);
        }

        /// <summary>
        /// Writes the basic properties
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="instance"></param>
        public void WriteProperties(XmlWriter writer, object instance) {
            foreach (PropertyInfo property in this.properties) {
                try {
                    WriteProperty(writer, instance, property);
                } catch (Exception e) {
                    Debug.Log($"Exception while writing property {property.Name}: {e.Message}");

                    throw e;
                }
            }

            // Write those properties that requires an element before closing
            WriteElementWriterProperties(writer, instance);
        }

        private void WriteProperty(XmlWriter writer, object instance, PropertyInfo property) {
            if (!TypeUtils.IsVariableProperty(property)) {
                return;
            }

            // Must have the Persist attribute
            object[] attributes = property.GetCustomAttributes(typeof(Persist), false);
            if (attributes.Length == 0) {
                // No Persist attribute
                return;
            }

            // Must have a writer
            bool written = false;
            if (this.attributeWriterMap.TryGetValue(property.PropertyType, out PropertyWriter propWriter)) {
                // Invokes the property writer
                propWriter(writer, property, instance);
                written = true;
            }

            if (this.elementWriterMap.ContainsKey(property.PropertyType)) {
                // This means that the property requires an element writer
                // We only write elements later
                this.requiresElementWriterProperties.Add(property);
                written = true;
            }

            // At this point, it's not written yet. We check if the property is IAcademiaSerializable.
            // If it is, we add it to serializableProperties
            if (!written) {
                if (property.PropertyType.GetInterface("IAcademiaSerializable") != null) {
                    this.serializableProperties.Add(property);
                    written = true;
                }
            }

            Assertion.IsTrue(written, property.Name + " was not written"); // Might forget the writer
        }

        /// <summary>
        /// Ends the element for the instance
        /// </summary>
        /// <param name="writer"></param>
        public void End(XmlWriter writer) {
            writer.WriteEndElement();
        }

        private void WriteElementWriterProperties(XmlWriter writer, object instance) {
            for (int i = 0; i < this.requiresElementWriterProperties.Count; ++i) {
                PropertyInfo current = this.requiresElementWriterProperties[i];
                this.elementWriterMap[current.PropertyType](writer, current, instance); // Invokes the delegate
            }

            this.requiresElementWriterProperties.Clear();
        }

        private static void WriteAsAttribute(XmlWriter writer, PropertyInfo property, object instance) {
            object value = property.GetGetMethod().Invoke(instance, null);
            if (value != null) {
                writer.SafeWriteAttributeString(property.Name, value.ToString());
            }
        }

        private static void WriteInt(XmlWriter writer, PropertyInfo property, object instance) {
            int value = (int) property.GetGetMethod().Invoke(instance, null);
            writer.SafeWriteAttributeString(property.Name, value.ToString(NumberFormatInfo.InvariantInfo));
        }

        private static void WriteFloat(XmlWriter writer, PropertyInfo property, object instance) {
            float value = (float) property.GetGetMethod().Invoke(instance, null);
            writer.SafeWriteAttributeString(property.Name, value.ToString(NumberFormatInfo.InvariantInfo));
        }

        private static void WriteDateTime(XmlWriter writer, PropertyInfo property, object instance) {
            DateTime value = (DateTime)property.GetGetMethod().Invoke(instance, null);
            writer.SafeWriteAttributeString(property.Name, value.Ticks.ToString());
        }

        private static void WriteVector3(XmlWriter writer, PropertyInfo property, object instance) {
            object value = property.GetGetMethod().Invoke(instance, null);
            if (value != null) {
                Vector3 vec = (Vector3) value;
                writer.WriteStartElement(property.Name);
                writer.SafeWriteAttributeString("x", vec.x.ToString(CultureInfo.InvariantCulture.NumberFormat));
                writer.SafeWriteAttributeString("y", vec.y.ToString(CultureInfo.InvariantCulture.NumberFormat));
                writer.SafeWriteAttributeString("z", vec.z.ToString(CultureInfo.InvariantCulture.NumberFormat));
                writer.WriteEndElement();
            }
        }

        private static void WriteColor(XmlWriter writer, PropertyInfo property, object instance) {
            object value = property.GetGetMethod().Invoke(instance, null);
            if (value == null) {
                return;
            }

            Color color = (Color) value;
            writer.WriteStartElement(property.Name);
            writer.SafeWriteAttributeString("r", color.r.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.SafeWriteAttributeString("g", color.g.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.SafeWriteAttributeString("b", color.b.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.SafeWriteAttributeString("a", color.a.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.WriteEndElement();
        }

        private static void WriteContributionType(XmlWriter writer, PropertyInfo property, object instance) {
            object value = property.GetGetMethod().Invoke(instance, null);
            if (value == null) {
                // No value. No need to write.
                return;
            }

            ContributionType contributionType = (ContributionType)value;
            writer.SafeWriteAttributeString(property.Name, contributionType.id);
        }

        private static void WriteStatus(XmlWriter writer, PropertyInfo property, object instance) {
            object value = property.GetGetMethod().Invoke(instance, null);
            if (value == null) {
                // No value. No need to write.
                return;
            }

            Status status = (Status)value;
            writer.SafeWriteAttributeString(property.Name, status.id.ToString());
        }

        private static void WriteIntArray(XmlWriter writer, PropertyInfo property, object instance) {
            object value = property.GetGetMethod().Invoke(instance, null);
            if (value != null) {
                int[] array = (int[]) value;

                writer.WriteStartElement(property.Name);
                for (int i = 0; i < array.Length; ++i) {
                    WriteElement(writer, array[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
                }

                writer.WriteEndElement();
            }
        }

        public const string ELEMENT = "Element";
        public const string VALUE = "Value";

        private static void WriteElement(XmlWriter writer, string value) {
            writer.WriteStartElement(ELEMENT);
            writer.SafeWriteAttributeString(VALUE, value);
            writer.WriteEndElement();
        }
    }
}