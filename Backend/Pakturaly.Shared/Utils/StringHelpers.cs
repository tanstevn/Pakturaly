using System.Text.Json;

namespace Pakturaly.Shared.Utils {
    public static class StringHelpers {
        public static string ToCamelCase(this string value) {
            return JsonNamingPolicy.CamelCase
                .ConvertName(value);
        }
    }
}
