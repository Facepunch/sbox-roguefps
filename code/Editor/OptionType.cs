using Editor;
using System;

namespace QuickSwitcher;

[Flags]
public enum OptionType
{
	Asset,
	Action,

	All = Asset | Action
};

public static class OptionTypeExtension
{
	public static Color GetColor( this OptionType type )
	{
		return type switch
		{
			OptionType.Asset => Theme.White,
			OptionType.Action => Theme.Primary,
			_ => Color.White
		};
	}
}
