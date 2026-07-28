using System.Collections.Generic;

namespace RobloxFlagInjector
{
    public class FlagPreset
    {
        public string Name { get; set; } = "";
        public FlagValueType ValueType { get; set; }
        public string SuggestedValue { get; set; } = "";
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Since Sept 29, 2025, Roblox only applies Fast Flags that appear on its
    /// official server-side Allowlist — everything else set locally is simply
    /// ignored (not punished, per Roblox's own FAQ, just inert). This list
    /// mirrors that official allowlist as of the last time it was checked.
    /// The allowlist changes over time, so treat this as a snapshot, not a
    /// guarantee — Roblox can add or remove entries at any point.
    /// Source: https://devforum.roblox.com/t/allowlist-for-local-client-configuration-via-fast-flags/3966569
    /// </summary>
    public static class FlagPresets
    {
        public static List<FlagPreset> Common = new()
        {
            // Geometry
            new FlagPreset
            {
                Name = "DFIntCSGLevelOfDetailSwitchingDistance",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "300",
                Description = "[Allowlisted] CSG level-of-detail switch distance."
            },
            new FlagPreset
            {
                Name = "DFIntCSGLevelOfDetailSwitchingDistanceL12",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "300",
                Description = "[Allowlisted] CSG LOD switch distance (level 1→2)."
            },
            new FlagPreset
            {
                Name = "DFIntCSGLevelOfDetailSwitchingDistanceL23",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "300",
                Description = "[Allowlisted] CSG LOD switch distance (level 2→3)."
            },
            new FlagPreset
            {
                Name = "DFIntCSGLevelOfDetailSwitchingDistanceL34",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "300",
                Description = "[Allowlisted] CSG LOD switch distance (level 3→4)."
            },

            // Rendering
            new FlagPreset
            {
                Name = "FFlagHandleAltEnterFullscreenManually",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Custom handling of Alt+Enter fullscreen toggle."
            },
            new FlagPreset
            {
                Name = "DFFlagTextureQualityOverrideEnabled",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Enables manual texture quality override."
            },
            new FlagPreset
            {
                Name = "DFIntTextureQualityOverride",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "3",
                Description = "[Allowlisted] Sets texture quality level (used with the flag above)."
            },
            new FlagPreset
            {
                Name = "FIntDebugForceMSAASamples",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "0",
                Description = "[Allowlisted] Forces a specific MSAA sample count (0 = off)."
            },
            new FlagPreset
            {
                Name = "DFFlagDisableDPIScale",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Disables DPI scaling of the client UI."
            },
            new FlagPreset
            {
                Name = "FFlagDebugGraphicsPreferD3D11",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Forces the Direct3D 11 rendering backend."
            },
            new FlagPreset
            {
                Name = "FFlagDebugGraphicsPreferVulkan",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Forces the Vulkan rendering backend."
            },
            new FlagPreset
            {
                Name = "FFlagDebugGraphicsPreferOpenGL",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Forces the OpenGL rendering backend."
            },
            new FlagPreset
            {
                Name = "FFlagDebugSkyGray",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Renders the sky as flat gray (debug visual)."
            },
            new FlagPreset
            {
                Name = "DFFlagDebugPauseVoxelizer",
                ValueType = FlagValueType.Boolean,
                SuggestedValue = "True",
                Description = "[Allowlisted] Pauses terrain voxelization (debug/perf)."
            },
            new FlagPreset
            {
                Name = "DFIntDebugFRMQualityLevelOverride",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "1",
                Description = "[Allowlisted] Overrides the Future Rendering Method quality level."
            },
            new FlagPreset
            {
                Name = "FIntFRMMaxGrassDistance",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "150",
                Description = "[Allowlisted] Max draw distance for grass (Future Rendering Method)."
            },
            new FlagPreset
            {
                Name = "FIntFRMMinGrassDistance",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "0",
                Description = "[Allowlisted] Min draw distance for grass (Future Rendering Method)."
            },

            // User Interface
            new FlagPreset
            {
                Name = "FIntGrassMovementReducedMotionFactor",
                ValueType = FlagValueType.Integer,
                SuggestedValue = "0",
                Description = "[Allowlisted] Reduces grass movement animation (motion sensitivity)."
            },
        };
    }
}
