using System;

namespace Deckify.Modules.Colors
{
    [Serializable]
    public class ColorModel
    {
        public string Name { get; set; }
        public string HexCode { get; set; } // e.g. "#FF0000"
        public string Group { get; set; }

        public ColorModel() { }

        public ColorModel(string name, string hexCode, string group = "Default")
        {
            Name = name;
            HexCode = hexCode;
            Group = group;
        }

        public override string ToString()
        {
            return Name; // For simple displaying in ListBox
        }
    }
}
