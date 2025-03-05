using System.Numerics;
using OneOf;

namespace VZ_Socket
{
    public class VzType
    {
        public OneOf<float, string, bool, Vector3> Value { get; set; }

        /// <summary>
        /// Infers the datatype through the string
        /// </summary>
        ///
        public VzType(string value)
        {
            string s = value.Trim();
            if (attemptToConvertToBool(s)) return;
            if (attemptToConvertToVector3(s)) return;
            if (attemptToConvertToFloat(s)) return;
            Value = s;
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
                    catch { 
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
        private bool attemptToConvertToFloat(string s) {
            if (float.TryParse(s, out float floatResult)) {
                Value = s;
                return true;
            };
            return false;
        }
    }
}
