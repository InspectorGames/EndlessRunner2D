// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Collections.Generic;
// ReSharper disable All
namespace Doozy.Runtime.UIManager.Containers
{
    public partial class UIView
    {
        public static IEnumerable<UIView> GetViews(UIViewId.InGame id) => GetViews(nameof(UIViewId.InGame), id.ToString());
        public static void Show(UIViewId.InGame id, bool instant = false) => Show(nameof(UIViewId.InGame), id.ToString(), instant);
        public static void Hide(UIViewId.InGame id, bool instant = false) => Hide(nameof(UIViewId.InGame), id.ToString(), instant);

        public static IEnumerable<UIView> GetViews(UIViewId.MainMenu id) => GetViews(nameof(UIViewId.MainMenu), id.ToString());
        public static void Show(UIViewId.MainMenu id, bool instant = false) => Show(nameof(UIViewId.MainMenu), id.ToString(), instant);
        public static void Hide(UIViewId.MainMenu id, bool instant = false) => Hide(nameof(UIViewId.MainMenu), id.ToString(), instant);

        public static IEnumerable<UIView> GetViews(UIViewId.Start id) => GetViews(nameof(UIViewId.Start), id.ToString());
        public static void Show(UIViewId.Start id, bool instant = false) => Show(nameof(UIViewId.Start), id.ToString(), instant);
        public static void Hide(UIViewId.Start id, bool instant = false) => Hide(nameof(UIViewId.Start), id.ToString(), instant);
        public static IEnumerable<UIView> GetViews(UIViewId.Tutorial id) => GetViews(nameof(UIViewId.Tutorial), id.ToString());
        public static void Show(UIViewId.Tutorial id, bool instant = false) => Show(nameof(UIViewId.Tutorial), id.ToString(), instant);
        public static void Hide(UIViewId.Tutorial id, bool instant = false) => Hide(nameof(UIViewId.Tutorial), id.ToString(), instant);
    }
}

namespace Doozy.Runtime.UIManager
{
    public partial class UIViewId
    {
        public enum InGame
        {
            Game,
            GameOver,
            Pause
        }

        public enum MainMenu
        {
            Home,
            Settings
        }

        public enum Start
        {
            Splash
        }
        public enum Tutorial
        {
            EndTutorial,
            ExplainBars,
            ExplainExcavatorFuel,
            ExplainFastFall,
            ExplainJump,
            ExplainMinecartFuel,
            ExplainObstacle,
            ExplainUseExcavatorFuel,
            ExplainWall,
            Start
        }    
    }
}