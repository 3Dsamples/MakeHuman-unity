//==============================================================================================
/*!レイヤー定義.
	@file  Layer
*/
//==============================================================================================
using UnityEngine;
using System;

namespace KS {
    public enum e_LayerId {
        Default = 0,
        RenderIcon = 23,
        Window = 24,
        BG = 25,
        Fade = 26,
        SystemWindow = 27,
        ProgressBar = 28,
        Home = 29,
        Profile = 30,
    };

    [Flags]
    public enum e_Layer {
        Default = 1 << (int)e_LayerId.Default,
        RenderIcon = 1 << (int)e_LayerId.RenderIcon,
        Window = 1 << (int)e_LayerId.Window,
        BG = 1 << (int)e_LayerId.BG,
        Fade = 1 << (int)e_LayerId.Fade,
        SystemWindow = 1 << (int)e_LayerId.SystemWindow,
        ProgressBar = 1 << (int)e_LayerId.ProgressBar,
        Home = 1 << (int)e_LayerId.Home,
        Profile = 1 << (int)e_LayerId.Profile,
    };
}