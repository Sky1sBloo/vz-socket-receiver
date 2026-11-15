using System.Numerics;
using OneOf;

namespace VZ_Sky
{
    /// <summary>
    /// Class for managing type system of Vz Connection
    /// </summary>
    ///
    public class VzType
    {
        /// <summary>
        /// The underlying typed value.
        /// </summary>
        public OneOf<float, string, bool, Vector3> Value { get; set; }

        /// <summary>
        /// Creates a new VzType with the given value.
        /// </summary>
        /// <param name="value">The value to store.</param>
        public VzType(OneOf<float, string, bool, Vector3> value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new empty VzType.
        /// </summary>
        public VzType()
        {
        }

        /// <summary>
        /// Infers the datatype through the string
        /// </summary>
        public void InferTypeFromString(string str)
        {
            string s = str.Trim();
            if (attemptToConvertToBool(s)) return;
            if (attemptToConvertToVector3(s)) return;
            if (attemptToConvertToFloat(s)) return;
            Value = s;

        }

        /// <summary>
        /// Retrieves the value to a void returnable function
        /// Equivalent of OneOf Switch
        /// </summary>
        public void GetValue(Action<float> f, Action<string> s, Action<bool> b, Action<Vector3> v)
        {
            Value.Switch(f, s, b, v);
        }

        /// <summary>
        /// Retrieves the value a returnable function
        /// Equivalent of OneOf Match 
        /// </summary>
        public TResult GetValue<TResult>(Func<float, TResult> f, Func<string, TResult> s, Func<bool, TResult> b, Func<Vector3, TResult> v)
        {
            return Value.Match(f, s, b, v);
        }

        /// <summary>
        /// Attempts to get the float value of the type
        /// </summary>
        ///
        /// <param name="value"> Returns the float value </param>
        ///
        /// <returns>True if the operation is successful.</returns>
        public bool GetFloat(out float value)
        {
            value = 0f;
            return Value.TryPickT0(out value, out _);
        }

        /// <summary>
        /// Attempts to get the stringvalue of the type
        /// </summary>
        ///
        /// <param name="value"> Returns the string value </param>
        ///
        /// <returns>True if the operation is successful.</returns>
        public bool GetString(out string value)
        {
            return Value.TryPickT1(out value, out _);
        }

        /// <summary>
        /// Attempts to get the boolean of the type
        /// </summary>
        ///
        /// <param name="value"> Returns the bool value </param>
        ///
        /// <returns>True if the operation is successful.</returns>
        public bool GetBool(out bool value)
        {
            return Value.TryPickT2(out value, out _);
        }

        /// <summary>
        /// Attempts to get the vector3 of the type
        /// </summary>
        ///
        /// <param name="value"> Returns the vector3 value </param>
        ///
        /// <returns>True if the operation is successful.</returns>
        public bool GetVector3(out Vector3 value)
        {
            return Value.TryPickT3(out value, out _);
        }


        /// <summary>
        /// Returns the string representation of the stored value.
        /// </summary>
        /// <returns>The string value.</returns>
        public override string ToString()
        {
            return Value.Match(
                    f => f.ToString(),
                    s => s,
                    b => b ? "1" : "0",  // somehow true or false doesn't work in juno
                    v => $"({v.X}, {v.Y}, {v.Z})"
                    );
        }

        /// <summary>
        /// Attempts to convert the string to a boolean
        /// and save it to value
        /// </summary>
        ///
        /// <returns>True if conversion is successful.</returns>
        private bool attemptToConvertToBool(string s)
        {
            if (s.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                Value = true;
                return true;
            }

            if (s.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                Value = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to convert the string to a Vector3 
        /// and save it to value
        /// </summary>
        ///
        /// <returns>True if conversion is successful.</returns>
        private bool attemptToConvertToVector3(string s)
        {
            if (s.StartsWith("(") && s.EndsWith(")"))
            {
                string inner = s[1..^1];
                string[] parts = inner.Split(',');
                if (parts.Length == 3 &&
                        float.TryParse(parts[0].Trim(), out float x) &&
                        float.TryParse(parts[1].Trim(), out float y) &&
                        float.TryParse(parts[2].Trim(), out float z))
                {
                    Value = new Vector3(
                            float.Parse(parts[0].Trim()),
                            float.Parse(parts[1].Trim()),
                            float.Parse(parts[2].Trim()));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to convert the string to a float 
        /// and save it to value
        /// </summary>
        ///
        /// <returns>True if conversion is successful.</returns>
        private bool attemptToConvertToFloat(string s)
        {
            if (float.TryParse(s, out float floatResult))
            {
                Value = floatResult;
                return true;
            };
            return false;
        }
    }
}
