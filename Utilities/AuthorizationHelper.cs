using Malama.Authorization;

namespace Malama.Utilities
{
    public static class RoleAttributeRequirementProvider
    {
        public static RoleAttributeRequirement GetRequirement(string featureName)
        {
            if (RoleAttributeConfig.RoleAttributeCombinations.TryGetValue(featureName, out var combinations))
            {
                return new RoleAttributeRequirement(combinations.ToArray());
            }

            return new RoleAttributeRequirement(Array.Empty<(string Role, string Attribute)>());
        }
    }

}
