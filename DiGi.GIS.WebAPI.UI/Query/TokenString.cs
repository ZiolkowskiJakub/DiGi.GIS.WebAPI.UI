using System;
using System.Text.Json;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Reads the session token out of the body a login or refresh answered with.
        /// <para>The user authentication service returns an anonymous object for both, which ASP.NET Core serializes camelCase, so the property is matched case-insensitively rather than against the spelling either side happens to use today.</para>
        /// <para>A body that is missing, is not an object, or carries no usable token collapses to <see langword="null"/>: to this application "no token" and "a body that made no sense" are the same failure to sign in, and neither is worth telling the visitor apart.</para>
        /// </summary>
        /// <param name="json">The response body to read. This value can be null.</param>
        /// <returns>The session token, or <see langword="null"/> when the body carries none.</returns>
        public static string? TokenString(this string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(json);

                if (jsonDocument.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return null;
                }

                foreach (JsonProperty jsonProperty in jsonDocument.RootElement.EnumerateObject())
                {
                    if (!string.Equals(jsonProperty.Name, "token", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (jsonProperty.Value.ValueKind != JsonValueKind.String)
                    {
                        return null;
                    }

                    string? tokenString = jsonProperty.Value.GetString();

                    return string.IsNullOrWhiteSpace(tokenString) ? null : tokenString;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
