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
        public OneOf<float, string, bool, Vector3> Value { get; set; }

        public VzType(OneOf<float, string, bool, Vector3> value)
        {
            Value = value;
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

        public override string ToString() {
            return Value.Match(
                    f => f.ToString(),
                    s => s,
                    b => b.ToString(),
                    v => $"({v.X}, {v.Y}, {v.Z})"
                    );
        }

        /// <summary>
        /// Attempts to convert the string to a boolean
        /// and save it to value
        /// </summary>
        ///
        /// <returns> True if convertion is successful
        ///
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
        /// <returns> True if convertion is successful
        ///
        private bool attemptToConvertToVector3(string s)
        {
            if (s.StartsWith("(") && s.EndsWith(")"))
            {
                string inner = s[1..^1];
                string[] parts = inner.Split(',');
                if (parts.Length == 3)
                {
                    try
                    {
                        Value = new Vector3(
                                float.Parse(parts[0].Trim()),
                                float.Parse(parts[1].Trim()),
                                float.Parse(parts[2].Trim()));
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to convert the string to a float 
        /// and save it to value
        /// </summary>
        ///
        /// <returns> True if convertion is successful
        ///
        private bool attemptToConvertToFloat(string s)
        {
            if (float.TryParse(s, out float floatResult))
            {
                Value = s;
                return true;
            };
            return false;
        }
    }
}
