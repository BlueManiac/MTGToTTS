using System;
using System.Threading.Tasks;

namespace DeckParser
{
    public class Options
    {
        public string BackUrl { get; set; } = "https://loremflickr.com/480/680";
        public string ResultPath { get; set; } = @"%USERPROFILE%\Documents\My Games\Tabletop Simulator\Saves\Saved Objects\Imported";

        public async Task Expand() {
            BackUrl = await WebUtil.ExpandUrl(BackUrl);
            ResultPath = Environment.ExpandEnvironmentVariables(ResultPath); 
        }
    }
}