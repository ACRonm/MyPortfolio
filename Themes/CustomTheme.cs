using MudBlazor;

namespace MyPortfolio.Themes
{
    public class CustomTheme : MudTheme
    {
        public CustomTheme()
        {
            Typography = new Typography()
            {
                Default = new Default()
                {
					FontFamily = new[] { "Inter" },
				},
                Button = new Button()
                {
                    FontFamily = new[] { "Jetbrains Mono" },
                },
                H1 = new H1() { FontSize = "2.5rem" },
                H2 = new H2() { FontSize = "2rem" },
                H3 = new H3() { FontSize = "1.75rem" },
                H4 = new H4() { FontSize = "1.5rem" },
                Body1 = new Body1() { FontSize = "medium" },
                Body2 = new Body2() { FontSize = "small" },

            };

            PaletteLight = new PaletteLight()
            {
                Background = "#f5f5f5",
                Primary = "#8B0000",
                Secondary = "#8B0000",
                TextPrimary = "#333",
                AppbarBackground = "#f5f5f5",
                Tertiary = "rgba(174,145,164,0.4)"
            };

            PaletteDark = new PaletteDark()
            {
                Background = "#1A1A27",
                AppbarBackground = "#1A1A27",
                Primary = "#FFC701",
                TextPrimary = "#CBD6ED",
                Secondary = "#FFC701",
                Tertiary = "rgba(45,40,38,0.4)"
            };
        }
    }
}