﻿using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.EscapeMenu.UI
{
    [GenerateTypedNameReferences]
    public partial class OptionsMenu : DefaultWindow
    {
        public OptionsMenu()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            Tabs.SetTabTitle(0, Loc.GetString("ui-options-tab-graphics"));
            Tabs.SetTabTitle(1, Loc.GetString("ui-options-tab-controls"));
            Tabs.SetTabTitle(2, Loc.GetString("ui-options-tab-audio"));
        }
    }
}
