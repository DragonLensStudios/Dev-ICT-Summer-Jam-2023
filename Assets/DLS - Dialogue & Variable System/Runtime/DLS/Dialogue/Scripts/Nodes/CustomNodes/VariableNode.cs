using System;
using UnityEngine;
using XNode;
using DLS.Core;

namespace DLS.Dialogue
{
    /// <summary>
    /// Represents a variable node in a dialogue graph.
    /// </summary>
    public class VariableNode : BaseNode
    {
        [Input] public Connection input;
        [Output] public Connection exitTrue;
        [Output] public Connection exitFalse;
        [SerializeField] protected string variableName;
        [SerializeField] protected VariableType variableType;
        [SerializeField] protected string serializedVariableValue;
        [SerializeField] protected Operator operatorType;

        private object variableValue;

        /// <summary>
        /// Gets or sets the name of the variable associated with this node.
        /// </summary>
        public string VariableName { get => variableName; set => variableName = value; }

        /// <summary>
        /// Gets or sets the type of the variable associated with this node.
        /// </summary>
        public VariableType VariableType { get => variableType; set => variableType = value; }

        /// <summary>
        /// Gets or sets the value of the variable associated with this node.
        /// </summary>
        public object VariableValue
        {
            get
            {
                switch (variableType)
                {
                    case VariableType.Int:
                        int.TryParse(serializedVariableValue, out int intValue);
                        return intValue;
                    case VariableType.Long:
                        long.TryParse(serializedVariableValue, out long longValue);
                        return longValue;
                    case VariableType.Short:
                        short.TryParse(serializedVariableValue, out short shortValue);
                        return shortValue;
                    case VariableType.Double:
                        double.TryParse(serializedVariableValue, out double doubleValue);
                        return doubleValue;
                    case VariableType.Decimal:
                        decimal.TryParse(serializedVariableValue, out decimal decimalValue);
                        return decimalValue;
                    case VariableType.Float:
                        float.TryParse(serializedVariableValue, out float floatValue);
                        return floatValue;
                    case VariableType.Bool:
                        bool.TryParse(serializedVariableValue, out bool boolValue);
                        return boolValue;
                    case VariableType.String:
                        return serializedVariableValue;
                    case VariableType.Vector2:
                        string[] vector2Values = serializedVariableValue.Split(',');
                        if (vector2Values.Length == 2 && float.TryParse(vector2Values[0], out float x) && float.TryParse(vector2Values[1], out float y))
                        {
                            return new ComparableVector2(new Vector2(x, y));
                        }
                        return null;
                    case VariableType.Vector3:
                        string[] vector3Values = serializedVariableValue.Split(',');
                        if (vector3Values.Length == 3 && float.TryParse(vector3Values[0], out float x3) && float.TryParse(vector3Values[1], out float y3) && float.TryParse(vector3Values[2], out float z))
                        {
                            return new ComparableVector3(new Vector3(x3, y3, z));
                        }
                        return null;
                    case VariableType.DateTime:
                        DateTime.TryParse(serializedVariableValue, out DateTime dateTimeValue);
                        return dateTimeValue;
                    default:
                        return null;
                }
            }
            set
            {
                switch (variableType)
                {
                    case VariableType.Int:
                        serializedVariableValue = ((int)value).ToString();
                        break;
                    case VariableType.Long:
                        serializedVariableValue = ((long)value).ToString();
                        break;
                    case VariableType.Short:
                        serializedVariableValue = ((short)value).ToString();
                        break;
                    case VariableType.Double:
                        serializedVariableValue = ((double)value).ToString();
                        break;
                    case VariableType.Decimal:
                        serializedVariableValue = ((decimal)value).ToString();
                        break;
                    case VariableType.Float:
                        serializedVariableValue = ((float)value).ToString();
                        break;
                    case VariableType.Bool:
                        serializedVariableValue = ((bool)value).ToString();
                        break;
                    case VariableType.String:
                        serializedVariableValue = (string)value;
                        break;
                    case VariableType.Vector2:
                        ComparableVector2 vector2Value = (ComparableVector2)value;
                        serializedVariableValue = $"{vector2Value.Value.x},{vector2Value.Value.y}";
                        break;
                    case VariableType.Vector3:
                        ComparableVector3 vector3Value = (ComparableVector3)value;
                        serializedVariableValue = $"{vector3Value.Value.x},{vector3Value.Value.y},{vector3Value.Value.z}";
                        break;
                    case VariableType.DateTime:
                        serializedVariableValue = ((DateTime)value).ToString("o"); // ISO 8601 format
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the operator type for comparing the variable value.
        /// </summary>
        public Operator OperatorType { get => operatorType; set => operatorType = value; }

        /// <summary>
        /// Returns the value of the specified output port.
        /// </summary>
        /// <param name="port">The output port to retrieve the value from.</param>
        /// <returns>The value of the output port.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
