using System;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace GeneralsBot {
    public class JsonContractResolver : DefaultContractResolver {
        protected override string ResolvePropertyName(string propertyName) {
            var s = propertyName.Split('_').ToList().Select(Uppercase).Aggregate((c, n) => c + n);
            Console.WriteLine(s);
            return s;
        }

        private string Uppercase(string word) {
            if (string.IsNullOrEmpty(word)) return string.Empty;

            return char.ToUpper(word[0]) + word.Substring(1);
        }
    }
}
