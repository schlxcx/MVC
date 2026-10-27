using System.Drawing;

namespace MeChatness.Services;

public static class Generator
{
    static readonly Random random = new Random();
    static readonly List<string> darkColors =
    [
        "#000000", // Black
        "#1E293B", // Slate
        "#1F2937", // Dark gray
        "#374151", // Gray
        "#1E3A8A", // Dark blue
        "#1D4ED8", // Blue
        "#312E81", // Indigo
        "#5B21B6", // Purple
        "#6B21A8", // Dark purple
        "#9D174D", // Dark pink
        "#991B1B", // Dark red
        "#B91C1C", // Red
        "#92400E", // Brown
        "#854D0E", // Dark yellow
        "#14532D", // Dark green
        "#166534", // Green
        "#065F46", // Emerald
        "#115E59", // Teal
        "#155E75", // Cyan
        "#0C4A6E"  // Dark cyan
    ];
    static readonly List<string> lightColors =
    [
        "#FFFFFF", // White
        "#F9FAFB", // Very light gray
        "#E5E7EB", // Light gray
        "#D1D5DB", // Gray
        "#BFDBFE", // Light blue
        "#93C5FD", // Sky blue
        "#C7D2FE", // Light indigo
        "#DDD6FE", // Lavender
        "#E9D5FF", // Light purple
        "#FBCFE8", // Light pink
        "#FDA4AF", // Rose
        "#FECACA", // Light red
        "#FED7AA", // Light orange
        "#FDE68A", // Yellow
        "#FEF08A", // Light yellow
        "#BBF7D0", // Light green
        "#86EFAC", // Green
        "#A7F3D0", // Mint
        "#99F6E4", // Turquoise
        "#A5F3FC"  // Light cyan
    ];

    public static string GetRandomLightHexColor() => lightColors[random.Next(lightColors.Count - 1)];
    public static string GetRandomDarkHexColor() => darkColors[random.Next(darkColors.Count - 1)];
}
