using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PuppeteerSharp.Mobile
{
    /// <summary>
    /// Device descriptors.
    /// </summary>
    public static class DeviceDescriptors
    {
        private static readonly Dictionary<DeviceDescriptorName, DeviceDescriptor> Devices = new Dictionary<DeviceDescriptorName, DeviceDescriptor>
        {
            [DeviceDescriptorName.BlackberryPlayBook] = new DeviceDescriptor
            {
                Name = "Blackberry PlayBook",
                UserAgent = "Mozilla/5.0 (PlayBook; U; RIM Tablet OS 2.1.0; en-US) AppleWebKit/536.2+ (KHTML like Gecko) Version/7.2.1.0 Safari/536.2+",
                ViewPort = new ViewPortOptions
                {
                    Width = 600,
                    Height = 1024,
                    DeviceScaleFactor = 1,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.BlackberryPlayBookLandscape] = new DeviceDescriptor
            {
                Name = "Blackberry PlayBook landscape",
                UserAgent = "Mozilla/5.0 (PlayBook; U; RIM Tablet OS 2.1.0; en-US) AppleWebKit/536.2+ (KHTML like Gecko) Version/7.2.1.0 Safari/536.2+",
                ViewPort = new ViewPortOptions
                {
                    Width = 1024,
                    Height = 600,
                    DeviceScaleFactor = 1,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.BlackBerryZ30] = new DeviceDescriptor
            {
                Name = "BlackBerry Z30",
                UserAgent = "Mozilla/5.0 (BB10; Touch) AppleWebKit/537.10+ (KHTML, like Gecko) Version/10.0.9.2372 Mobile Safari/537.10+",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 640,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.BlackBerryZ30Landscape] = new DeviceDescriptor
            {
                Name = "BlackBerry Z30 landscape",
                UserAgent = "Mozilla/5.0 (BB10; Touch) AppleWebKit/537.10+ (KHTML, like Gecko) Version/10.0.9.2372 Mobile Safari/537.10+",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.GalaxyNote3] = new DeviceDescriptor
            {
                Name = "Galaxy Note 3",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.3; en-us; SM-N900T Build/JSS15J) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 640,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.GalaxyNote3Landscape] = new DeviceDescriptor
            {
                Name = "Galaxy Note 3 landscape",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.3; en-us; SM-N900T Build/JSS15J) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.GalaxyNoteII] = new DeviceDescriptor
            {
                Name = "Galaxy Note II",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.1; en-us; GT-N7100 Build/JRO03C) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 640,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.GalaxyNoteIILandscape] = new DeviceDescriptor
            {
                Name = "Galaxy Note II landscape",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.1; en-us; GT-N7100 Build/JRO03C) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.GalaxySIII] = new DeviceDescriptor
            {
                Name = "Galaxy S III",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.0; en-us; GT-I9300 Build/IMM76D) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 640,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.GalaxySIIILandscape] = new DeviceDescriptor
            {
                Name = "Galaxy S III landscape",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.0; en-us; GT-I9300 Build/IMM76D) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.GalaxyS5] = new DeviceDescriptor
            {
                Name = "Galaxy S5",
                UserAgent = "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 640,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.GalaxyS5Landscape] = new DeviceDescriptor
            {
                Name = "Galaxy S5 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.GalaxyS8] = new DeviceDescriptor
            {
                Name = "Galaxy S8",
                UserAgent = "Mozilla/5.0 (Linux; Android 7.0; SM-G950U Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.84 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 740,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.GalaxyS8Landscape] = new DeviceDescriptor
            {
                Name = "Galaxy S8 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 7.0; SM-G950U Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.84 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 740,
                    Height = 360,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.GalaxyS9Plus] = new DeviceDescriptor
            {
                Name = "Galaxy S9+",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; SM-G965U Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.111 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 320,
                    Height = 658,
                    DeviceScaleFactor = 4.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.GalaxyS9PlusLandscape] = new DeviceDescriptor
            {
                Name = "Galaxy S9+ landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; SM-G965U Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.111 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 658,
                    Height = 320,
                    DeviceScaleFactor = 4.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.GalaxyTabS4] = new DeviceDescriptor
            {
                Name = "Galaxy Tab S4",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.1.0; SM-T837A) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.80 Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 712,
                    Height = 1138,
                    DeviceScaleFactor = 2.25,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.GalaxyTabS4Landscape] = new DeviceDescriptor
            {
                Name = "Galaxy Tab S4 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.1.0; SM-T837A) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.80 Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 1138,
                    Height = 712,
                    DeviceScaleFactor = 2.25,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPad] = new DeviceDescriptor
            {
                Name = "iPad",
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 768,
                    Height = 1024,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPadLandscape] = new DeviceDescriptor
            {
                Name = "iPad landscape",
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 1024,
                    Height = 768,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPadMini] = new DeviceDescriptor
            {
                Name = "iPad Mini",
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 768,
                    Height = 1024,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPadMiniLandscape] = new DeviceDescriptor
            {
                Name = "iPad Mini landscape",
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 1024,
                    Height = 768,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPadPro] = new DeviceDescriptor
            {
                Name = "iPad Pro",
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 1024,
                    Height = 1366,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPadProLandscape] = new DeviceDescriptor
            {
                Name = "iPad Pro landscape",
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 1366,
                    Height = 1024,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone4] = new DeviceDescriptor
            {
                Name = "iPhone 4",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 7_1_2 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D257 Safari/9537.53",
                ViewPort = new ViewPortOptions
                {
                    Width = 320,
                    Height = 480,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone4Landscape] = new DeviceDescriptor
            {
                Name = "iPhone 4 landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 7_1_2 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D257 Safari/9537.53",
                ViewPort = new ViewPortOptions
                {
                    Width = 480,
                    Height = 320,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone5] = new DeviceDescriptor
            {
                Name = "iPhone 5",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 320,
                    Height = 568,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone5Landscape] = new DeviceDescriptor
            {
                Name = "iPhone 5 landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 568,
                    Height = 320,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone6] = new DeviceDescriptor
            {
                Name = "iPhone 6",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 375,
                    Height = 667,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone6Landscape] = new DeviceDescriptor
            {
                Name = "iPhone 6 landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 667,
                    Height = 375,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone6Plus] = new DeviceDescriptor
            {
                Name = "iPhone 6 Plus",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 414,
                    Height = 736,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone6PlusLandscape] = new DeviceDescriptor
            {
                Name = "iPhone 6 Plus landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 736,
                    Height = 414,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone7] = new DeviceDescriptor
            {
                Name = "iPhone 7",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 375,
                    Height = 667,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone7Landscape] = new DeviceDescriptor
            {
                Name = "iPhone 7 landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 667,
                    Height = 375,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone7Plus] = new DeviceDescriptor
            {
                Name = "iPhone 7 Plus",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 414,
                    Height = 736,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone7PlusLandscape] = new DeviceDescriptor
            {
                Name = "iPhone 7 Plus landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 736,
                    Height = 414,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone8] = new DeviceDescriptor
            {
                Name = "iPhone 8",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 375,
                    Height = 667,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone8Landscape] = new DeviceDescriptor
            {
                Name = "iPhone 8 landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 667,
                    Height = 375,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone8Plus] = new DeviceDescriptor
            {
                Name = "iPhone 8 Plus",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 414,
                    Height = 736,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone8PlusLandscape] = new DeviceDescriptor
            {
                Name = "iPhone 8 Plus landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 736,
                    Height = 414,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhoneSE] = new DeviceDescriptor
            {
                Name = "iPhone SE",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 320,
                    Height = 568,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhoneSELandscape] = new DeviceDescriptor
            {
                Name = "iPhone SE landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 568,
                    Height = 320,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhoneX] = new DeviceDescriptor
            {
                Name = "iPhone X",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 375,
                    Height = 812,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhoneXLandscape] = new DeviceDescriptor
            {
                Name = "iPhone X landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 812,
                    Height = 375,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhoneXR] = new DeviceDescriptor
            {
                Name = "iPhone XR",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 414,
                    Height = 896,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhoneXRLandscape] = new DeviceDescriptor
            {
                Name = "iPhone XR landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 896,
                    Height = 414,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone11] = new DeviceDescriptor
            {
                Name = "iPhone 11",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 414,
                    Height = 828,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone11Landscape] = new DeviceDescriptor
            {
                Name = "iPhone 11 landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 828,
                    Height = 414,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone11Pro] = new DeviceDescriptor
            {
                Name = "iPhone 11 Pro",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 375,
                    Height = 812,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone11ProLandscape] = new DeviceDescriptor
            {
                Name = "iPhone 11 Pro landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 812,
                    Height = 375,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.IPhone11ProMax] = new DeviceDescriptor
            {
                Name = "iPhone 11 Pro Max",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 414,
                    Height = 896,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.IPhone11ProMaxLandscape] = new DeviceDescriptor
            {
                Name = "iPhone 11 Pro Max landscape",
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Mobile/15E148 Safari/604.1",
                ViewPort = new ViewPortOptions
                {
                    Width = 896,
                    Height = 414,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.JioPhone2] = new DeviceDescriptor
            {
                Name = "JioPhone 2",
                UserAgent = "Mozilla/5.0 (Mobile; LYF/F300B/LYF-F300B-001-01-15-130718-i;Android; rv:48.0) Gecko/48.0 Firefox/48.0 KAIOS/2.5",
                ViewPort = new ViewPortOptions
                {
                    Width = 240,
                    Height = 320,
                    DeviceScaleFactor = 1,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.JioPhone2Landscape] = new DeviceDescriptor
            {
                Name = "JioPhone 2 landscape",
                UserAgent = "Mozilla/5.0 (Mobile; LYF/F300B/LYF-F300B-001-01-15-130718-i;Android; rv:48.0) Gecko/48.0 Firefox/48.0 KAIOS/2.5",
                ViewPort = new ViewPortOptions
                {
                    Width = 320,
                    Height = 240,
                    DeviceScaleFactor = 1,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.KindleFireHDX] = new DeviceDescriptor
            {
                Name = "Kindle Fire HDX",
                UserAgent = "Mozilla/5.0 (Linux; U; en-us; KFAPWI Build/JDQ39) AppleWebKit/535.19 (KHTML, like Gecko) Silk/3.13 Safari/535.19 Silk-Accelerated=true",
                ViewPort = new ViewPortOptions
                {
                    Width = 800,
                    Height = 1280,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.KindleFireHDXLandscape] = new DeviceDescriptor
            {
                Name = "Kindle Fire HDX landscape",
                UserAgent = "Mozilla/5.0 (Linux; U; en-us; KFAPWI Build/JDQ39) AppleWebKit/535.19 (KHTML, like Gecko) Silk/3.13 Safari/535.19 Silk-Accelerated=true",
                ViewPort = new ViewPortOptions
                {
                    Width = 1280,
                    Height = 800,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.LGOptimusL70] = new DeviceDescriptor
            {
                Name = "LG Optimus L70",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.4.2; en-us; LGMS323 Build/KOT49I.MS32310c) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 384,
                    Height = 640,
                    DeviceScaleFactor = 1.25,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.LGOptimusL70Landscape] = new DeviceDescriptor
            {
                Name = "LG Optimus L70 landscape",
                UserAgent = "Mozilla/5.0 (Linux; U; Android 4.4.2; en-us; LGMS323 Build/KOT49I.MS32310c) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 384,
                    DeviceScaleFactor = 1.25,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.MicrosoftLumia550] = new DeviceDescriptor
            {
                Name = "Microsoft Lumia 550",
                UserAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 550) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.MicrosoftLumia950] = new DeviceDescriptor
            {
                Name = "Microsoft Lumia 950",
                UserAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 640,
                    DeviceScaleFactor = 4,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.MicrosoftLumia950Landscape] = new DeviceDescriptor
            {
                Name = "Microsoft Lumia 950 landscape",
                UserAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 4,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Nexus10] = new DeviceDescriptor
            {
                Name = "Nexus 10",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 10 Build/MOB31T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 800,
                    Height = 1280,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Nexus10Landscape] = new DeviceDescriptor
            {
                Name = "Nexus 10 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 10 Build/MOB31T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 1280,
                    Height = 800,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Nexus4] = new DeviceDescriptor
            {
                Name = "Nexus 4",
                UserAgent = "Mozilla/5.0 (Linux; Android 4.4.2; Nexus 4 Build/KOT49H) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 384,
                    Height = 640,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Nexus4Landscape] = new DeviceDescriptor
            {
                Name = "Nexus 4 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 4.4.2; Nexus 4 Build/KOT49H) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 384,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Nexus5] = new DeviceDescriptor
            {
                Name = "Nexus 5",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 360,
                    Height = 640,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Nexus5Landscape] = new DeviceDescriptor
            {
                Name = "Nexus 5 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 640,
                    Height = 360,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Nexus5X] = new DeviceDescriptor
            {
                Name = "Nexus 5X",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Nexus 5X Build/OPR4.170623.006) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 412,
                    Height = 732,
                    DeviceScaleFactor = 2.625,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Nexus5XLandscape] = new DeviceDescriptor
            {
                Name = "Nexus 5X landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Nexus 5X Build/OPR4.170623.006) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 732,
                    Height = 412,
                    DeviceScaleFactor = 2.625,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Nexus6] = new DeviceDescriptor
            {
                Name = "Nexus 6",
                UserAgent = "Mozilla/5.0 (Linux; Android 7.1.1; Nexus 6 Build/N6F26U) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 412,
                    Height = 732,
                    DeviceScaleFactor = 3.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Nexus6Landscape] = new DeviceDescriptor
            {
                Name = "Nexus 6 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 7.1.1; Nexus 6 Build/N6F26U) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 732,
                    Height = 412,
                    DeviceScaleFactor = 3.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Nexus6P] = new DeviceDescriptor
            {
                Name = "Nexus 6P",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Nexus 6P Build/OPP3.170518.006) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 412,
                    Height = 732,
                    DeviceScaleFactor = 3.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Nexus6PLandscape] = new DeviceDescriptor
            {
                Name = "Nexus 6P landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Nexus 6P Build/OPP3.170518.006) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 732,
                    Height = 412,
                    DeviceScaleFactor = 3.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Nexus7] = new DeviceDescriptor
            {
                Name = "Nexus 7",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 7 Build/MOB30X) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 600,
                    Height = 960,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Nexus7Landscape] = new DeviceDescriptor
            {
                Name = "Nexus 7 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 7 Build/MOB30X) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 960,
                    Height = 600,
                    DeviceScaleFactor = 2,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.NokiaLumia520] = new DeviceDescriptor
            {
                Name = "Nokia Lumia 520",
                UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)",
                ViewPort = new ViewPortOptions
                {
                    Width = 320,
                    Height = 533,
                    DeviceScaleFactor = 1.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.NokiaLumia520Landscape] = new DeviceDescriptor
            {
                Name = "Nokia Lumia 520 landscape",
                UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)",
                ViewPort = new ViewPortOptions
                {
                    Width = 533,
                    Height = 320,
                    DeviceScaleFactor = 1.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.NokiaN9] = new DeviceDescriptor
            {
                Name = "Nokia N9",
                UserAgent = "Mozilla/5.0 (MeeGo; NokiaN9) AppleWebKit/534.13 (KHTML, like Gecko) NokiaBrowser/8.5.0 Mobile Safari/534.13",
                ViewPort = new ViewPortOptions
                {
                    Width = 480,
                    Height = 854,
                    DeviceScaleFactor = 1,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.NokiaN9Landscape] = new DeviceDescriptor
            {
                Name = "Nokia N9 landscape",
                UserAgent = "Mozilla/5.0 (MeeGo; NokiaN9) AppleWebKit/534.13 (KHTML, like Gecko) NokiaBrowser/8.5.0 Mobile Safari/534.13",
                ViewPort = new ViewPortOptions
                {
                    Width = 854,
                    Height = 480,
                    DeviceScaleFactor = 1,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Pixel2] = new DeviceDescriptor
            {
                Name = "Pixel 2",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 411,
                    Height = 731,
                    DeviceScaleFactor = 2.625,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Pixel2Landscape] = new DeviceDescriptor
            {
                Name = "Pixel 2 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 731,
                    Height = 411,
                    DeviceScaleFactor = 2.625,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Pixel2XL] = new DeviceDescriptor
            {
                Name = "Pixel 2 XL",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Pixel 2 XL Build/OPD1.170816.004) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 411,
                    Height = 823,
                    DeviceScaleFactor = 3.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Pixel2XLLandscape] = new DeviceDescriptor
            {
                Name = "Pixel 2 XL landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Pixel 2 XL Build/OPD1.170816.004) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3765.0 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 823,
                    Height = 411,
                    DeviceScaleFactor = 3.5,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Pixel3] = new DeviceDescriptor
            {
                Name = "Pixel 3",
                UserAgent = "Mozilla/5.0 (Linux; Android 9; Pixel 3 Build/PQ1A.181105.017.A1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.158 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 393,
                    Height = 786,
                    DeviceScaleFactor = 2.75,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Pixel3Landscape] = new DeviceDescriptor
            {
                Name = "Pixel 3 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 9; Pixel 3 Build/PQ1A.181105.017.A1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.158 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 786,
                    Height = 393,
                    DeviceScaleFactor = 2.75,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
            [DeviceDescriptorName.Pixel4] = new DeviceDescriptor
            {
                Name = "Pixel 4",
                UserAgent = "Mozilla/5.0 (Linux; Android 10; Pixel 4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 353,
                    Height = 745,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = false,
                },
            },
            [DeviceDescriptorName.Pixel4Landscape] = new DeviceDescriptor
            {
                Name = "Pixel 4 landscape",
                UserAgent = "Mozilla/5.0 (Linux; Android 10; Pixel 4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Mobile Safari/537.36",
                ViewPort = new ViewPortOptions
                {
                    Width = 745,
                    Height = 353,
                    DeviceScaleFactor = 3,
                    IsMobile = true,
                    HasTouch = true,
                    IsLandscape = true,
                },
            },
        };

        private static readonly Lazy<IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor>> _readOnlyDevices =
            new Lazy<IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor>>(() => new ReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor>(Devices));

        /// <summary>
        /// Get the specified device description.
        /// </summary>
        /// <returns>The device descriptor.</returns>
        /// <param name="name">Device Name.</param>
        [Obsolete("Use Puppeteer.Devices instead")]
        public static DeviceDescriptor Get(DeviceDescriptorName name) => Devices[name];

        internal static IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor> ToReadOnly() => _readOnlyDevices.Value;
    }
}
