using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using vdo.trias;
using Xunit;

namespace DerMistkaefer.DvbLive.TriasCommunication.UnitTests
{
    /// <summary>
    /// Tests for the Trias Structure
    /// </summary>
    public class TriasStructureTests
    {
        /// <summary>
        /// Test if the Trias Structure is Serializable
        /// </summary>
        [Fact]
        public void TestTriasStructure()
        {
            var trias = new Trias();
            TestClass(trias, "");
        }

        private bool TestClass(object data, string errorPath)
        {
            if (!TestXmlStructure(data))
            {
                errorPath += $"{data.GetType().Name} -> ";
                TestFields(data, errorPath);
                return false;
            }

            return true;
        }

        private void TestFields(object data, string errorPath)
        {
            var testedFields = new List<bool>();
            foreach (var field in data.GetType().GetFields())
            { 
                var fieldIo = TestField(field, errorPath);
                testedFields.Add(fieldIo);
            }

            if (testedFields.Any(x => x))
            {
                throw new Exception(errorPath);
            }
        }

        private bool TestField(FieldInfo field, string errorPath)
        {
            var fieldType = field.FieldType;
            var attrbuteTypes = field.CustomAttributes
                .Where(atribute => atribute.AttributeType == typeof(XmlElementAttribute))
                .SelectMany(xmlAtrribute => xmlAtrribute.ConstructorArguments
                    .Where(arg => arg.ArgumentType == typeof(Type) && arg.Value != null)
                    .Select(typeArg => (Type)typeArg.Value!)
                );
            var types = new List<Type>
            {
                fieldType
            };
            types.AddRange(attrbuteTypes);
            var testedClass = new List<(Type type, bool result)>();
            foreach (var type in types.Where(type => type.Namespace == typeof(Trias).Namespace))
            {
                var elementType = type.GetElementType();
                var testType = type.GetElementType() ?? type; // Array/List -> Use Base Type
                var typeData = Activator.CreateInstance(testType);
                if (typeData != null)
                {
                    testedClass.Add((testType, TestClass(typeData, errorPath)));
                }
            }

            if (testedClass.Count == 0)
            {
                return false;
            }

            return testedClass.All(x => x.result); // IF Sub Element - Error then Field Error
        }

        private static bool TestXmlStructure(object data)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(data.GetType());
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }
    }
}
