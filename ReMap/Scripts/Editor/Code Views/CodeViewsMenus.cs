using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

using WindowUtility;

namespace CodeViews
{
    public enum MenuType
    {
        Large,
        Medium,
        Small
    }

    public class CodeViewsMenu
    {
        // Debug Only
        static string strRef = "strRef";
        static bool boolRef = true;
        static int intRef = 1;
        static Vector3 vecRef = new Vector3(0, 0, 0);
        //

        internal static FunctionRef EmptyFunctionRef = () => { };
        internal static FunctionRef[] EmptyFunctionRefArray = new FunctionRef[0];

        internal static Color GUI_SettingsColor = new Color(255f, 255f, 255f);

        internal static FunctionRef[] TipsMenu = new FunctionRef[]
        {
            () => OptionalTextInfo( "Ctrl + R = Refresh", "", null, MenuType.Medium ),
            () => OptionalTextInfo( "Ctrl + F = Search Code Window", "", null, MenuType.Medium )
        };

        internal static FunctionRef[] DevMenu = new FunctionRef[]
        {
            () => CreateMenu( CodeViewsWindow.DevMenuDebugInfo, EmptyFunctionRefArray, MenuType.Medium, "Hide Debug Info", "Show Debug Info", "Get infos from current window" )
        };

        internal static FunctionRef[] LargeFieldPreview = new FunctionRef[]
        {
            () => OptionalTextField( ref strRef, "text ref", "text field", null, MenuType.Large ),
            () => OptionalToggle( ref boolRef, "toggle ref", "toggle field", null, MenuType.Large ),
            () => OptionalIntField( ref intRef, "int ref", "int field", null, MenuType.Large ),
            () => OptionalVector3Field( ref vecRef, "vector ref", "vector field", null, MenuType.Large ),
            () => OptionalTextInfo( "info ref", "info field", null, MenuType.Large ),
            () => OptionalButton( "button ref", "button field", EmptyFunctionRef, null, MenuType.Large ),

            () => CreateMenu( "MediumFieldPreview", MediumFieldPreview, MenuType.Medium, "Medium Field Preview", "Medium Field Preview", "", false, false )
        };

        internal static FunctionRef[] MediumFieldPreview = new FunctionRef[]
        {
            () => OptionalTextField( ref strRef, "text ref", "text field", null, MenuType.Medium ),
            () => OptionalToggle( ref boolRef, "toggle ref", "toggle field", null, MenuType.Medium ),
            () => OptionalIntField( ref intRef, "int ref", "int field", null, MenuType.Medium ),
            () => OptionalVector3Field( ref vecRef, "vector ref", "vector field", null, MenuType.Medium ),
            () => OptionalTextInfo( "info ref", "info field", null, MenuType.Medium ),
            () => OptionalButton( "button ref", "button field", EmptyFunctionRef, null, MenuType.Medium ),

            () => CreateMenu( "SmallFieldPreview", SmallFieldPreview, MenuType.Small, "Small Field Preview", "Small Field Preview", "", false, false )
        };

        internal static FunctionRef[] SmallFieldPreview = new FunctionRef[]
        {
            () => OptionalTextField( ref strRef, "text ref", "text field", null, MenuType.Small ),
            () => OptionalToggle( ref boolRef, "toggle ref", "toggle field", null, MenuType.Small ),
            () => OptionalIntField( ref intRef, "int ref", "int field", null, MenuType.Small ),
            () => OptionalVector3Field( ref vecRef, "vector ref", "vector field", null, MenuType.Small ),
            () => OptionalTextInfo( "info ref", "info field", null, MenuType.Small ),
            () => OptionalButton( "button ref", "button field", EmptyFunctionRef, null, MenuType.Small )
        };

        internal static FunctionRef[] ColorPreview = ButtonColor();

        internal static void SharedFunctions()
        {
            CreateMenu(CodeViewsWindow.TipsMenu, TipsMenu, MenuType.Large, "Tips", "Tips", "");

#if RMAPDEV
            GUILayout.BeginHorizontal();
            Space(2); Separator(314);
            GUILayout.EndHorizontal();

            CreateMenu(CodeViewsWindow.DevMenu, DevMenu, MenuType.Large, "Dev Menu", "Dev Menu", "");
            CreateMenu("LargeFieldPreview", LargeFieldPreview, MenuType.Large, "Field Preview", "Field Preview", "");
            CreateMenu("NotificationColorPreview", ColorPreview, MenuType.Large, "Color Preview", "Color Preview", "");
#endif
        }

        // Dev Only
        internal static FunctionRef[] ButtonColor()
        {
            List<FunctionRef> list = new List<FunctionRef>();

            foreach (var color in CodeViewsWindow.Color_Array)
            {
                list.Add(() => OptionalButton(color.Key, "", () => { CodeViewsWindow.ephemeralMessage.AddToQueueMessage($"#RMAPDEV_MESSAGE_{color.Key}", $"{color.Key} Notification", 4, true, color.Value); }, null, MenuType.Medium));
            }

            return list.ToArray();
        }

        internal static void SelectionMenu()
        {
            CodeViewsMenu.CreateMenu(CodeViewsWindow.SelectionMenu, EmptyFunctionRefArray, MenuType.Large, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", true);
        }

        internal static MenuInit CreateMenu(string name, FunctionRef functionRef, MenuType menuType = MenuType.Large, string trueText = "", string falseText = "", string tooltip = "", bool refresh = false, bool enableSeparator = true, bool condition = true, bool isButton = false)
        {
            return CreateMenu(name, new FunctionRef[] { functionRef }, menuType, trueText, falseText, tooltip, refresh, enableSeparator, condition, isButton);
        }

        internal static MenuInit CreateMenu(string name, FunctionRef[] functionRef, MenuType menuType = MenuType.Large, string trueText = "", string falseText = "", string tooltip = "", bool refresh = false, bool enableSeparator = true, bool condition = true, bool isButton = false)
        {
            MenuInit menu;

            if (MenuInit.Exist(name))
            {
                menu = MenuInit.Find(name);
                menu.Content = functionRef;
            }
            else menu = new MenuInit(name, functionRef, menuType);

            if (!condition) return menu;

            menu.EnableSeparator = menu.IsButton ? false : enableSeparator;
            menu.IsButton = isButton;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent(menu.IsOpen ? trueText : falseText, tooltip);
            GUIContent buttonContentInfo = new GUIContent(menu.IsOpen ? CodeViewsWindow.enableLogo : CodeViewsWindow.disableLogo, tooltip);

            if (menuType != MenuType.Large) Space(4);

            GUILayout.BeginHorizontal();

            Space(menu.Space);

            if (GUILayout.Button(buttonContentInfo, buttonStyle, GUILayout.Height(20), GUILayout.Width(20)) || GUILayout.Button(buttonContent, buttonStyle, GUILayout.Height(20), GUILayout.Width(menu.Width)))
            {
                menu.IsOpen = !menu.IsOpen;

                if (menu.IsButton) Internal_FunctionInit(menu);

                if (refresh) CodeViewsWindow.Refresh();

                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            if (!menu.IsButton) Internal_FunctionInit(menu);

            return menu;
        }

        internal static void Internal_FunctionInit(MenuInit menu)
        {
            if (menu.Content.Length != 0 && menu.IsOpen || menu.IsButton)
            {
                foreach (FunctionRef functionRef in menu.Content) functionRef();

                if (!menu.EnableSeparator) return;

                if (menu.MenuType == MenuType.Medium || menu.MenuType == MenuType.Small) GUILayout.BeginHorizontal();
                Space(menu.Space); Separator(menu.SeparatorWidth);
                if (menu.MenuType == MenuType.Medium || menu.MenuType == MenuType.Small) GUILayout.EndHorizontal();
            }
            else if (menu.MenuType == MenuType.Large) Space(8);
        }


        //   ██████╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗███████╗
        //  ██╔═══██╗██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║██╔════╝
        //  ██║   ██║██████╔╝   ██║   ██║██║   ██║██╔██╗ ██║███████╗
        //  ██║   ██║██╔═══╝    ██║   ██║██║   ██║██║╚██╗██║╚════██║
        //  ╚██████╔╝██║        ██║   ██║╚██████╔╝██║ ╚████║███████║
        //   ╚═════╝ ╚═╝        ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝
        internal static void OptionalTextField(ref string reference, string text = "text field", string tooltip = "", bool? condition = null, MenuType menuType = MenuType.Large)
        {
            if (condition != null && !condition.Value) return;

            Space(4);

            float space = 0, labelSpace = 0, fieldSpace = 0;

            switch (menuType)
            {
                case MenuType.Large:
                    space = 2;
                    labelSpace = 92;
                    fieldSpace = 220;
                    break;
                case MenuType.Medium:
                    space = 12;
                    labelSpace = 92;
                    fieldSpace = 210;
                    break;
                case MenuType.Small:
                    space = 25;
                    labelSpace = 89;
                    fieldSpace = 200;
                    break;
            }

            GUILayout.BeginHorizontal();
            Space(space);

            WindowUtility.WindowUtility.CreateTextField(ref reference, text, tooltip, labelSpace, fieldSpace);
            GUILayout.EndHorizontal();
        }

        internal static void OptionalToggle(ref bool reference, string text = "toggle", string tooltip = "", bool? condition = null, MenuType menuType = MenuType.Large)
        {
            if (condition != null && !condition.Value) return;

            Space(4);

            float space = 0, labelSpace = 0;

            switch (menuType)
            {
                case MenuType.Large:
                    space = 2;
                    labelSpace = 292;
                    break;
                case MenuType.Medium:
                    space = 12;
                    labelSpace = 282;
                    break;
                case MenuType.Small:
                    space = 25;
                    labelSpace = 269;
                    break;
            }

            GUILayout.BeginHorizontal();
            Space(space);

            WindowUtility.WindowUtility.CreateToggle(ref reference, text, tooltip, labelSpace);
            GUILayout.EndHorizontal();
        }

        internal static void OptionalIntField(ref int reference, string text = "int field", string tooltip = "", bool? condition = null, MenuType menuType = MenuType.Large)
        {
            if (condition != null && !condition.Value) return;

            Space(4);

            float space = 0, labelSpace = 0, fieldSpace = 0;

            switch (menuType)
            {
                case MenuType.Large:
                    space = 2;
                    labelSpace = 226;
                    fieldSpace = 86;
                    break;
                case MenuType.Medium:
                    space = 12;
                    labelSpace = 224;
                    fieldSpace = 78;
                    break;
                case MenuType.Small:
                    space = 25;
                    labelSpace = 219;
                    fieldSpace = 70;
                    break;
            }

            GUILayout.BeginHorizontal();
            Space(space);

            WindowUtility.WindowUtility.CreateIntField(ref reference, text, tooltip, labelSpace, fieldSpace);
            GUILayout.EndHorizontal();
        }

        internal static void OptionalVector3Field(ref Vector3 reference, string text = "vector3 field", string tooltip = "", bool? condition = null, MenuType menuType = MenuType.Large)
        {
            if (condition != null && !condition.Value) return;

            Space(4);

            float space = 0, labelSpace = 0, fieldSpace = 0;

            switch (menuType)
            {
                case MenuType.Large:
                    space = 2;
                    labelSpace = 102;
                    fieldSpace = 210;
                    break;
                case MenuType.Medium:
                    space = 12;
                    labelSpace = 92;
                    fieldSpace = 210;
                    break;
                case MenuType.Small:
                    space = 25;
                    labelSpace = 79;
                    fieldSpace = 210;
                    break;
            }

            GUILayout.BeginHorizontal();
            Space(space);

            WindowUtility.WindowUtility.CreateVector3Field(ref reference, text, tooltip, labelSpace, fieldSpace);
            GUILayout.EndHorizontal();
        }

        internal static void OptionalTextInfo(string text = "text", string tooltip = "", bool? condition = null, MenuType menuType = MenuType.Large, bool centered = false)
        {
            if (condition != null && !condition.Value) return;

            Space(4);

            float space = 0, labelSpace = 0;

            switch (menuType)
            {
                case MenuType.Large:
                    space = 2;
                    labelSpace = 316;
                    break;
                case MenuType.Medium:
                    space = 12;
                    labelSpace = 302;
                    break;
                case MenuType.Small:
                    space = 25;
                    labelSpace = 289;
                    break;
            }

            GUILayout.BeginHorizontal();
            Space(space);

            if (centered)
            {
                WindowUtility.WindowUtility.CreateTextInfoCentered(text, tooltip, labelSpace);
            }
            else WindowUtility.WindowUtility.CreateTextInfo(text, tooltip, labelSpace);

            GUILayout.EndHorizontal();
        }

        internal static void OptionalButton(string text = "button", string tooltip = "", FunctionRef functionRef = null, bool? condition = null, MenuType menuType = MenuType.Large, bool refresh = false)
        {
            if (condition != null && !condition.Value) return;

            Space(4);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            float space = 0, labelSpace = 0;

            switch (menuType)
            {
                case MenuType.Large:
                    space = 2;
                    labelSpace = 314;
                    break;
                case MenuType.Medium:
                    space = 12;
                    labelSpace = 305;
                    break;
                case MenuType.Small:
                    space = 25;
                    labelSpace = 292;
                    break;
            }

            GUILayout.BeginHorizontal();
            Space(space);

            if (WindowUtility.WindowUtility.CreateButton(text, tooltip, functionRef, labelSpace, 20) && refresh) CodeViewsWindow.Refresh();
            GUILayout.EndHorizontal();
        }

        internal static void OptionalAdvancedOption()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            Space(4);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            WindowUtility.WindowUtility.CreateButton("Check All", "", () => CheckOptionalAdvancedOption(true), 156);

            WindowUtility.WindowUtility.CreateButton("Uncheck All", "", () => CheckOptionalAdvancedOption(false), 156);
            GUILayout.EndHorizontal();

            foreach (ObjectType objectType in Helper.ObjectsToHide.Keys)
            {
                if (!Helper.GetBoolFromObjectsToHide(objectType)) continue;

                GUILayout.BeginHorizontal();
                bool value = Helper.GenerateObjects[objectType];
                string name = Helper.GetObjNameWithEnum(objectType);
                OptionalToggle(ref value, $"Build {name}", value ? $"Disable {name}" : $"Enable {name}");
                GUILayout.EndHorizontal();

                if (Helper.GenerateObjects[objectType] != value)
                {
                    Helper.GenerateObjects[objectType] = value;
                    CodeViewsWindow.Refresh();

                    GUILayout.EndVertical();
                    return;
                }
            }

            GUILayout.EndVertical();
        }

        internal static void VerifyGenerateObjects()
        {
            int idx = 0; float progress = 0.0f;
            int min = 0; int max = Helper.GenerateObjects.Keys.Count;

            foreach (ObjectType objectType in Helper.GenerateObjects.Keys)
            {
                EditorUtility.DisplayProgressBar($"Object Verification {min++}/{max}", $"Processing... ({Helper.GetObjNameWithEnum(objectType)})", progress);

                progress += 1.0f / max;

                if (CodeViewsWindow.IsHidden(objectType))
                {
                    Helper.ObjectsToHide[objectType] = false;
                    continue;
                }
                else Helper.ObjectsToHide[objectType] = true;

                if (Helper.GenerateObjects[objectType]) idx++;
            }

            EditorUtility.ClearProgressBar();

            CodeViewsWindow.objectTypeInSceneCount = idx;
        }

        private static bool isFirstOpen = true;

        internal static void OptionalAdditionalCodeOption()
        {
            if (!Helper.IsValid(AdditionalCodeTab.additionalCode)) AdditionalCodeTab.additionalCode = AdditionalCodeTab.FindAdditionalCode();

            GUILayout.BeginVertical();

            OptionalTextInfo($"Head Code", "", null, MenuType.Medium);
            Space(2);

            if (CodeViewsWindow.ShowFunctionEnable())
            {
                foreach (AdditionalCodeContent content in AdditionalCodeTab.additionalCode.HeadContent.Content)
                {
                    CreateMenu($"{content.Name}_HeadContent", () => AdditionalCodeBoolChange(content.Name, "HeadContent", ref CodeViewsWindow.additionalCodeHead, AdditionalCodeTab.additionalCode.HeadContent.Content), MenuType.Small, content.Name, content.Name, "", false, false, true, true);
                }
            }
            else OptionalTextInfo($"\"Show Squirrel Function\" is disable", "", null, MenuType.Small);

            Space(1);
            OptionalTextInfo($"In-Block Code", "", null, MenuType.Medium);
            Space(2);

            foreach (AdditionalCodeContent content in AdditionalCodeTab.additionalCode.InBlockContent.Content)
            {
                CreateMenu($"{content.Name}_InBlockContent", () => AdditionalCodeBoolChange(content.Name, "InBlockContent", ref CodeViewsWindow.additionalCodeInBlock, AdditionalCodeTab.additionalCode.InBlockContent.Content), MenuType.Small, content.Name, content.Name, "", false, false, true, true);
            }

            Space(1);
            OptionalTextInfo($"Below Code", "", null, MenuType.Medium);
            Space(2);

            if (CodeViewsWindow.ShowFunctionEnable())
            {
                foreach (AdditionalCodeContent content in AdditionalCodeTab.additionalCode.BelowContent.Content)
                {
                    CreateMenu($"{content.Name}_BelowContent", () => AdditionalCodeBoolChange(content.Name, "BelowContent", ref CodeViewsWindow.additionalCodeBelow, AdditionalCodeTab.additionalCode.BelowContent.Content), MenuType.Small, content.Name, content.Name, "", false, false, true, true);
                }
            }
            else OptionalTextInfo($"\"Show Squirrel Function\" is disable", "", null, MenuType.Small);

            GUILayout.EndVertical();

            if (isFirstOpen)
            {
                isFirstOpen = false;

                foreach (AdditionalCodeContent content in AdditionalCodeTab.additionalCode.HeadContent.Content)
                {
                    CreateMenu($"{content.Name}_HeadContent", () => AdditionalCodeBoolChange(content.Name, "HeadContent", ref CodeViewsWindow.additionalCodeHead, AdditionalCodeTab.additionalCode.HeadContent.Content), MenuType.Small, content.Name, content.Name, "", false, false, true, true);
                }

                foreach (AdditionalCodeContent content in AdditionalCodeTab.additionalCode.InBlockContent.Content)
                {
                    CreateMenu($"{content.Name}_InBlockContent", () => AdditionalCodeBoolChange(content.Name, "InBlockContent", ref CodeViewsWindow.additionalCodeInBlock, AdditionalCodeTab.additionalCode.InBlockContent.Content), MenuType.Small, content.Name, content.Name, "", false, false, true, true);
                }

                foreach (AdditionalCodeContent content in AdditionalCodeTab.additionalCode.BelowContent.Content)
                {
                    CreateMenu($"{content.Name}_BelowContent", () => AdditionalCodeBoolChange(content.Name, "BelowContent", ref CodeViewsWindow.additionalCodeBelow, AdditionalCodeTab.additionalCode.BelowContent.Content), MenuType.Small, content.Name, content.Name, "", false, false, true, true);
                }

                foreach (string type in AdditionalCodeTab.contentType)
                {
                    MenuInit menu = MenuInit.Find($"{AdditionalCodeTab.emptyContentStr}_{type}");

                    if (Helper.IsValid(menu))
                    {
                        menu.IsOpen = true;
                    }
                }
            }
        }

        private static void AdditionalCodeBoolChange(string name, string type, ref string codeRef, List<AdditionalCodeContent> contents)
        {
            if (contents.Count == 1)
            {
                MenuInit umenu = MenuInit.Find($"{name}_{type}");

                if (Helper.IsValid(umenu)) umenu.IsOpen = true;

                return;
            }

            foreach (AdditionalCodeContent content in contents)
            {
                MenuInit menu = MenuInit.Find($"{content.Name}_{type}");

                if (!Helper.IsValid(menu)) return;

                if (name == content.Name)
                {
                    menu.IsOpen = true;

                    codeRef = content.Code;
                }
                else menu.IsOpen = false;
            }

            CodeViewsWindow.Refresh();
        }

        private static void CheckOptionalAdvancedOption(bool value)
        {
            Helper.ForceSetBoolToGenerateObjects(Helper.GetAllObjectType(), value);
            CodeViewsWindow.Refresh();
        }

        internal static void Space(float value)
        {
            GUILayout.Space(value);
        }

        internal static void Separator(int space)
        {
            GUI.backgroundColor = GUI_SettingsColor;
            GUILayout.Box("", GUILayout.Width(space), GUILayout.Height(4));
            GUI.backgroundColor = Color.white;
        }
    }

    public class MenuInit
    {
        public static Dictionary<string, MenuInit> MenuDictionary = new Dictionary<string, MenuInit>();
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public bool IsButton { get; set; }
        public bool EnableSeparator { get; set; }
        public FunctionRef[] Content { get; set; }
        public MenuType MenuType { get; set; }

        public int Space { get; set; }
        public int Width { get; set; }
        public int SeparatorWidth { get; set; }

        public MenuInit(string name, FunctionRef[] functionRef, MenuType menuType)
        {
            if (Exist(name)) return;

            Name = name;
            IsOpen = false;
            EnableSeparator = true;
            Content = functionRef;
            MenuType = menuType;

            switch (menuType)
            {
                case MenuType.Large:
                    Space = 2;
                    Width = 292;
                    SeparatorWidth = 312;
                    break;
                case MenuType.Medium:
                    Space = 12;
                    Width = 282;
                    SeparatorWidth = 304;
                    break;
                case MenuType.Small:
                    Space = 25;
                    Width = 269;
                    SeparatorWidth = 291;
                    break;
            }

            MenuDictionary[name] = this;
        }

        public static bool Exist(string name)
        {
            return Helper.IsValid(Find(name));
        }

        public static MenuInit Find(string name)
        {
            return MenuDictionary.TryGetValue(name, out MenuInit menu) ? menu : null;
        }

        public static bool IsEnable(string name)
        {
            return Find(name)?.IsOpen ?? false;
        }

        public static void SetBool(string name, bool value)
        {
            MenuInit menu = Find(name);

            if (Helper.IsValid(menu))
            {
                menu.IsOpen = value;
            }
        }
    }
}
