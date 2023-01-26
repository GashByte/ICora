//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.

using System.Collections.Generic;
using System.Text;

namespace XFP.Impact_Ultimate.Controls.Game.Utils
{
    /// <summary>
    /// Toke from Snap.Data.Utility
    /// </summary>
    public class CommandLineBuilder
    {
        private const char WhiteSpace = ' ';
        private readonly Dictionary<string, string?> options = new();

        public CommandLineBuilder AppendIf(string name, bool condition, object? value = null)
        {
            return condition ? Append(name, value) : this;
        }

        public CommandLineBuilder Append(string name, object? value = null)
        {
            options.Add(name, value?.ToString());
            return this;
        }

        public string Build()
        {
            return ToString();
        }

        public override string ToString()
        {
            StringBuilder s = new();
            foreach ((string key, string? value) in options)
            {
                s.Append(WhiteSpace);
                s.Append(key);
                if (!string.IsNullOrEmpty(value))
                {
                    s.Append(WhiteSpace);
                    s.Append(value);
                }
            }
            return s.ToString();
        }
    }
}
